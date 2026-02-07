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
    public LeasesController(IUnitOfWork uow) => _uow = uow;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LeaseDto>>> GetAll()
    {
        var leases = await _uow.Leases.GetAllAsync();
        return Ok(leases.Select(MapToDto));
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<LeaseDto>>> GetActive()
    {
        var leases = await _uow.Leases.GetActiveLeaseAsync();
        return Ok(leases.Select(MapToDto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LeaseDto>> GetById(Guid id)
    {
        var lease = await _uow.Leases.GetWithPaymentsAsync(id);
        if (lease == null) return NotFound();
        return Ok(MapToDto(lease));
    }

    [HttpGet("unit/{unitId}")]
    public async Task<ActionResult<IEnumerable<LeaseDto>>> GetByUnit(Guid unitId)
    {
        var leases = await _uow.Leases.GetByUnitIdAsync(unitId);
        return Ok(leases.Select(MapToDto));
    }

    [HttpGet("tenant/{tenantId}")]
    public async Task<ActionResult<IEnumerable<LeaseDto>>> GetByTenant(Guid tenantId)
    {
        var leases = await _uow.Leases.GetByTenantIdAsync(tenantId);
        return Ok(leases.Select(MapToDto));
    }

    [HttpGet("expiring")]
    public async Task<ActionResult<IEnumerable<LeaseDto>>> GetExpiring([FromQuery] int days = 90)
    {
        var leases = await _uow.Leases.GetExpiringLeasesAsync(days);
        return Ok(leases.Select(MapToDto));
    }

    [HttpPost]
    public async Task<ActionResult<LeaseDto>> Create([FromBody] LeaseCreateDto dto)
    {
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
        var unit = await _uow.Units.GetByIdAsync(dto.UnitId);
        if (unit != null) unit.Status = UnitStatus.Vermietet;

        await _uow.Leases.AddAsync(lease);
        await _uow.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = lease.Id }, MapToDto(lease));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] LeaseUpdateDto dto)
    {
        var lease = await _uow.Leases.GetByIdAsync(id);
        if (lease == null) return NotFound();

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

        // Wenn Vertrag beendet, Einheit freigeben
        if (dto.Status == LeaseStatus.Beendet)
        {
            var unit = await _uow.Units.GetByIdAsync(lease.UnitId);
            if (unit != null) unit.Status = UnitStatus.Leer;
        }

        await _uow.Leases.UpdateAsync(lease);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var lease = await _uow.Leases.GetByIdAsync(id);
        if (lease == null) return NotFound();

        // Einheit freigeben
        var unit = await _uow.Units.GetByIdAsync(lease.UnitId);
        if (unit != null) unit.Status = UnitStatus.Leer;

        await _uow.Leases.DeleteAsync(id);
        await _uow.SaveChangesAsync();
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
