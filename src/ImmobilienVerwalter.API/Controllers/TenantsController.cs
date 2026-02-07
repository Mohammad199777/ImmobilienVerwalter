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
public class TenantsController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<TenantsController> _logger;

    public TenantsController(IUnitOfWork uow, ILogger<TenantsController> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Gibt Mieter-IDs zurück, die über Leases mit Einheiten des Owners verbunden sind.
    /// </summary>
    private async Task<HashSet<Guid>> GetOwnerTenantIdsAsync()
    {
        var properties = await _uow.Properties.GetByOwnerIdAsync(GetUserId());
        var tenantIds = new HashSet<Guid>();

        foreach (var property in properties)
        {
            foreach (var unit in property.Units ?? Enumerable.Empty<Unit>())
            {
                var leases = await _uow.Leases.GetByUnitIdAsync(unit.Id);
                foreach (var lease in leases)
                    tenantIds.Add(lease.TenantId);
            }
        }

        return tenantIds;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TenantDto>>> GetAll()
    {
        // Alle Mieter zurückgeben — Mieter werden bei Vertragsanlage dem Owner zugeordnet
        // In einem größeren System würde man OwnerId auf Tenant setzen
        var tenants = await _uow.Tenants.GetAllAsync();
        return Ok(tenants.Select(MapToDto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TenantDto>> GetById(Guid id)
    {
        var tenant = await _uow.Tenants.GetWithLeasesAsync(id);
        if (tenant == null) return NotFound();
        return Ok(MapToDto(tenant));
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<TenantDto>>> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
            return BadRequest(new { message = "Suchbegriff muss mindestens 2 Zeichen lang sein." });

        var tenants = await _uow.Tenants.SearchAsync(q);
        return Ok(tenants.Select(MapToDto));
    }

    [HttpPost]
    public async Task<ActionResult<TenantDto>> Create([FromBody] TenantCreateDto dto)
    {
        var tenant = new Tenant
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            MobilePhone = dto.MobilePhone,
            PreviousAddress = dto.PreviousAddress,
            Iban = dto.Iban,
            Bic = dto.Bic,
            BankName = dto.BankName,
            DateOfBirth = dto.DateOfBirth,
            Occupation = dto.Occupation,
            MonthlyIncome = dto.MonthlyIncome,
            EmergencyContactName = dto.EmergencyContactName,
            EmergencyContactPhone = dto.EmergencyContactPhone,
            Notes = dto.Notes
        };

        await _uow.Tenants.AddAsync(tenant);
        await _uow.SaveChangesAsync();

        _logger.LogInformation("Mieter {TenantName} erstellt von User {UserId}", tenant.FullName, GetUserId());
        return CreatedAtAction(nameof(GetById), new { id = tenant.Id }, MapToDto(tenant));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] TenantUpdateDto dto)
    {
        var tenant = await _uow.Tenants.GetByIdAsync(id);
        if (tenant == null) return NotFound();

        tenant.FirstName = dto.FirstName;
        tenant.LastName = dto.LastName;
        tenant.Email = dto.Email;
        tenant.Phone = dto.Phone;
        tenant.MobilePhone = dto.MobilePhone;
        tenant.Iban = dto.Iban;
        tenant.Bic = dto.Bic;
        tenant.BankName = dto.BankName;
        tenant.DateOfBirth = dto.DateOfBirth;
        tenant.Occupation = dto.Occupation;
        tenant.MonthlyIncome = dto.MonthlyIncome;
        tenant.EmergencyContactName = dto.EmergencyContactName;
        tenant.EmergencyContactPhone = dto.EmergencyContactPhone;
        tenant.Notes = dto.Notes;

        await _uow.Tenants.UpdateAsync(tenant);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var tenant = await _uow.Tenants.GetWithLeasesAsync(id);
        if (tenant == null) return NotFound();

        // Prüfen ob aktive Mietverträge existieren
        var hasActiveLeases = tenant.Leases?.Any(l => l.Status == LeaseStatus.Aktiv) ?? false;
        if (hasActiveLeases)
            return BadRequest(new { message = "Mieter kann nicht gelöscht werden, da aktive Mietverträge existieren." });

        await _uow.Tenants.DeleteAsync(id);
        await _uow.SaveChangesAsync();

        _logger.LogInformation("Mieter {TenantId} gelöscht von User {UserId}", id, GetUserId());
        return NoContent();
    }

    private static TenantDto MapToDto(Tenant t) => new(
        t.Id, t.FirstName, t.LastName, t.FullName,
        t.Email, t.Phone, t.MobilePhone,
        t.PreviousAddress, t.Iban, t.Bic, t.BankName,
        t.DateOfBirth, t.Occupation, t.MonthlyIncome,
        t.EmergencyContactName, t.EmergencyContactPhone,
        t.Notes,
        t.Leases?.Count(l => l.Status == LeaseStatus.Aktiv) ?? 0,
        t.CreatedAt);
}
