using ImmobilienVerwalter.API.DTOs;
using ImmobilienVerwalter.Core.Entities;
using ImmobilienVerwalter.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImmobilienVerwalter.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UnitsController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    public UnitsController(IUnitOfWork uow) => _uow = uow;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UnitDto>>> GetAll()
    {
        var units = await _uow.Units.GetAllAsync();
        return Ok(units.Select(MapToDto));
    }

    [HttpGet("property/{propertyId}")]
    public async Task<ActionResult<IEnumerable<UnitDto>>> GetByProperty(Guid propertyId)
    {
        var units = await _uow.Units.GetByPropertyIdAsync(propertyId);
        return Ok(units.Select(MapToDto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UnitDto>> GetById(Guid id)
    {
        var unit = await _uow.Units.GetWithLeaseAsync(id);
        if (unit == null) return NotFound();
        return Ok(MapToDto(unit));
    }

    [HttpGet("vacant")]
    public async Task<ActionResult<IEnumerable<UnitDto>>> GetVacant([FromQuery] Guid? propertyId)
    {
        var units = await _uow.Units.GetVacantUnitsAsync(propertyId);
        return Ok(units.Select(MapToDto));
    }

    [HttpPost]
    public async Task<ActionResult<UnitDto>> Create([FromBody] UnitCreateDto dto)
    {
        var unit = new Unit
        {
            Name = dto.Name,
            Description = dto.Description,
            Floor = dto.Floor,
            Area = dto.Area,
            Rooms = dto.Rooms,
            Type = dto.Type,
            TargetRent = dto.TargetRent,
            PropertyId = dto.PropertyId,
            Status = UnitStatus.Leer
        };

        await _uow.Units.AddAsync(unit);
        await _uow.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = unit.Id }, MapToDto(unit));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UnitUpdateDto dto)
    {
        var unit = await _uow.Units.GetByIdAsync(id);
        if (unit == null) return NotFound();

        unit.Name = dto.Name;
        unit.Description = dto.Description;
        unit.Floor = dto.Floor;
        unit.Area = dto.Area;
        unit.Rooms = dto.Rooms;
        unit.Type = dto.Type;
        unit.Status = dto.Status;
        unit.TargetRent = dto.TargetRent;

        await _uow.Units.UpdateAsync(unit);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _uow.Units.DeleteAsync(id);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    private static UnitDto MapToDto(Unit u)
    {
        var activeLease = u.Leases?.FirstOrDefault(l => l.Status == LeaseStatus.Aktiv);
        TenantSummaryDto? tenantDto = activeLease?.Tenant != null
            ? new TenantSummaryDto(activeLease.Tenant.Id, activeLease.Tenant.FullName, activeLease.Tenant.Email)
            : null;

        return new UnitDto(
            u.Id, u.Name, u.Description, u.Floor,
            u.Area, u.Rooms, u.Type, u.Status,
            u.TargetRent, u.PropertyId, u.Property?.Name ?? "",
            tenantDto, u.CreatedAt);
    }
}
