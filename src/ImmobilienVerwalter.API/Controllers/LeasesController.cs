using System.Security.Claims;
using ImmobilienVerwalter.API.DTOs;
using ImmobilienVerwalter.Core.Entities;
using ImmobilienVerwalter.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImmobilienVerwalter.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LeasesController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<LeasesController> _logger;

    public LeasesController(IUnitOfWork uow, ILogger<LeasesController> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private async Task<HashSet<Guid>> GetOwnerPropertyIdsAsync()
    {
        var properties = await _uow.Properties.GetByOwnerIdAsync(GetUserId());
        return properties.Select(p => p.Id).ToHashSet();
    }

    private async Task<bool> IsOwnerOfUnit(Guid unitId)
    {
        var unit = await _uow.Units.GetByIdAsync(unitId);
        if (unit == null) return false;
        var property = await _uow.Properties.GetByIdAsync(unit.PropertyId);
        return property != null && property.OwnerId == GetUserId();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LeaseDto>>> GetAll()
    {
        var leases = await _uow.Leases.GetAllAsync();
        var propertyIds = await GetOwnerPropertyIdsAsync();
        var filtered = leases.Where(l => l.Unit?.PropertyId != null && propertyIds.Contains(l.Unit.PropertyId));
        return Ok(filtered.Select(MapToDto));
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<LeaseDto>>> GetActive()
    {
        var leases = await _uow.Leases.GetActiveLeaseAsync();
        var propertyIds = await GetOwnerPropertyIdsAsync();
        var filtered = leases.Where(l => l.Unit?.PropertyId != null && propertyIds.Contains(l.Unit.PropertyId));
        return Ok(filtered.Select(MapToDto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LeaseDto>> GetById(Guid id)
    {
        var lease = await _uow.Leases.GetWithPaymentsAsync(id);
        if (lease == null) return NotFound();
        if (!await IsOwnerOfUnit(lease.UnitId)) return NotFound();
        return Ok(MapToDto(lease));
    }

    [HttpGet("unit/{unitId}")]
    public async Task<ActionResult<IEnumerable<LeaseDto>>> GetByUnit(Guid unitId)
    {
        if (!await IsOwnerOfUnit(unitId)) return NotFound();
        var leases = await _uow.Leases.GetByUnitIdAsync(unitId);
        return Ok(leases.Select(MapToDto));
    }

    [HttpGet("tenant/{tenantId}")]
    public async Task<ActionResult<IEnumerable<LeaseDto>>> GetByTenant(Guid tenantId)
    {
        var leases = await _uow.Leases.GetByTenantIdAsync(tenantId);
        var propertyIds = await GetOwnerPropertyIdsAsync();
        var filtered = leases.Where(l => l.Unit?.PropertyId != null && propertyIds.Contains(l.Unit.PropertyId));
        return Ok(filtered.Select(MapToDto));
    }

    [HttpGet("expiring")]
    public async Task<ActionResult<IEnumerable<LeaseDto>>> GetExpiring([FromQuery] int days = 90)
    {
        var leases = await _uow.Leases.GetExpiringLeasesAsync(days);
        var propertyIds = await GetOwnerPropertyIdsAsync();
        var filtered = leases.Where(l => l.Unit?.PropertyId != null && propertyIds.Contains(l.Unit.PropertyId));
        return Ok(filtered.Select(MapToDto));
    }

    [HttpPost]
    public async Task<ActionResult<LeaseDto>> Create([FromBody] LeaseCreateDto dto)
    {
        // Ownership prüfen
        if (!await IsOwnerOfUnit(dto.UnitId))
            return Forbid();

        // Prüfen ob Unit existiert und leer ist
        var unit = await _uow.Units.GetWithLeaseAsync(dto.UnitId);
        if (unit == null)
            return BadRequest(new { message = "Einheit nicht gefunden." });

        // Prüfen ob bereits ein aktiver Vertrag existiert
        var existingLeases = await _uow.Leases.GetByUnitIdAsync(dto.UnitId);
        if (existingLeases.Any(l => l.Status == LeaseStatus.Aktiv))
            return BadRequest(new { message = "Diese Einheit hat bereits einen aktiven Mietvertrag." });

        // Prüfen ob Mieter existiert
        if (!await _uow.Tenants.ExistsAsync(dto.TenantId))
            return BadRequest(new { message = "Mieter nicht gefunden." });

        var lease = new Lease
        {
            TenantId = dto.TenantId,
            UnitId = dto.UnitId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            ColdRent = dto.ColdRent,
            AdditionalCosts = dto.AdditionalCosts,
            DepositAmount = dto.DepositAmount,
            NoticePeriodMonths = dto.NoticePeriodMonths,
            PaymentDayOfMonth = dto.PaymentDayOfMonth,
            Notes = dto.Notes,
            Status = LeaseStatus.Aktiv
        };

        // Einheit als vermietet markieren
        unit.Status = UnitStatus.Vermietet;

        await _uow.Leases.AddAsync(lease);
        await _uow.SaveChangesAsync();

        _logger.LogInformation("Mietvertrag erstellt: Unit {UnitId}, Tenant {TenantId}", dto.UnitId, dto.TenantId);
        return CreatedAtAction(nameof(GetById), new { id = lease.Id }, MapToDto(lease));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] LeaseUpdateDto dto)
    {
        var lease = await _uow.Leases.GetByIdAsync(id);
        if (lease == null) return NotFound();
        if (!await IsOwnerOfUnit(lease.UnitId)) return NotFound();

        var previousStatus = lease.Status;

        lease.ColdRent = dto.ColdRent;
        lease.AdditionalCosts = dto.AdditionalCosts;
        lease.DepositAmount = dto.DepositAmount;
        lease.DepositPaid = dto.DepositPaid;
        lease.DepositStatus = dto.DepositStatus;
        lease.Status = dto.Status;
        lease.EndDate = dto.EndDate;
        lease.TerminationDate = dto.TerminationDate;
        lease.MoveOutDate = dto.MoveOutDate;
        lease.NoticePeriodMonths = dto.NoticePeriodMonths;
        lease.PaymentDayOfMonth = dto.PaymentDayOfMonth;
        lease.Notes = dto.Notes;

        // Einheitsstatus synchronisieren
        if (dto.Status == LeaseStatus.Beendet && previousStatus != LeaseStatus.Beendet)
        {
            var unit = await _uow.Units.GetByIdAsync(lease.UnitId);
            if (unit != null) unit.Status = UnitStatus.Leer;
        }
        else if (dto.Status == LeaseStatus.Aktiv && previousStatus != LeaseStatus.Aktiv)
        {
            var unit = await _uow.Units.GetByIdAsync(lease.UnitId);
            if (unit != null) unit.Status = UnitStatus.Vermietet;
        }

        await _uow.Leases.UpdateAsync(lease);
        await _uow.SaveChangesAsync();

        _logger.LogInformation("Mietvertrag {LeaseId} aktualisiert, Status: {Status}", id, dto.Status);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var lease = await _uow.Leases.GetByIdAsync(id);
        if (lease == null) return NotFound();
        if (!await IsOwnerOfUnit(lease.UnitId)) return NotFound();

        // Einheit freigeben
        var unit = await _uow.Units.GetByIdAsync(lease.UnitId);
        if (unit != null) unit.Status = UnitStatus.Leer;

        await _uow.Leases.DeleteAsync(id);
        await _uow.SaveChangesAsync();

        _logger.LogInformation("Mietvertrag {LeaseId} gelöscht", id);
        return NoContent();
    }

    private static LeaseDto MapToDto(Lease l) => new(
        l.Id, l.TenantId, l.Tenant?.FullName ?? "",
        l.UnitId, l.Unit?.Name ?? "", l.Unit?.Property?.Name ?? "",
        l.StartDate, l.EndDate,
        l.ColdRent, l.AdditionalCosts, l.TotalRent,
        l.DepositAmount, l.DepositPaid, l.DepositStatus,
        l.Status, l.NoticePeriodMonths, l.PaymentDayOfMonth,
        l.IsActive, l.Notes, l.CreatedAt);
}
