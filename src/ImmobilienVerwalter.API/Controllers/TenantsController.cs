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
    public TenantsController(IUnitOfWork uow) => _uow = uow;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TenantDto>>> GetAll()
    {
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
        await _uow.Tenants.DeleteAsync(id);
        await _uow.SaveChangesAsync();
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
