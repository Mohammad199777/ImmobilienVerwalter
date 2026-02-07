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
public class UnitsController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<UnitsController> _logger;

    public UnitsController(IUnitOfWork uow, ILogger<UnitsController> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private async Task<bool> IsOwnerOfProperty(Guid propertyId)
    {
        var property = await _uow.Properties.GetByIdAsync(propertyId);
        return property != null && property.OwnerId == GetUserId();
    }

    private async Task<bool> IsOwnerOfUnit(Unit unit)
    {
        var property = await _uow.Properties.GetByIdAsync(unit.PropertyId);
        return property != null && property.OwnerId == GetUserId();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UnitDto>>> GetAll()
    {
        var ownerId = GetUserId();
        var properties = await _uow.Properties.GetByOwnerIdAsync(ownerId);
        var propertyIds = properties.Select(p => p.Id).ToHashSet();

        var allUnits = await _uow.Units.GetAllAsync();
        var userUnits = allUnits.Where(u => propertyIds.Contains(u.PropertyId));
        return Ok(userUnits.Select(MapToDto));
    }

    [HttpGet("property/{propertyId}")]
    public async Task<ActionResult<IEnumerable<UnitDto>>> GetByProperty(Guid propertyId)
    {
        if (!await IsOwnerOfProperty(propertyId))
            return NotFound();

        var units = await _uow.Units.GetByPropertyIdAsync(propertyId);
        return Ok(units.Select(MapToDto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UnitDto>> GetById(Guid id)
    {
        var unit = await _uow.Units.GetWithLeaseAsync(id);
        if (unit == null || !await IsOwnerOfUnit(unit))
            return NotFound();
        return Ok(MapToDto(unit));
    }

    [HttpGet("vacant")]
    public async Task<ActionResult<IEnumerable<UnitDto>>> GetVacant([FromQuery] Guid? propertyId)
    {
        if (propertyId.HasValue && !await IsOwnerOfProperty(propertyId.Value))
            return NotFound();

        var units = await _uow.Units.GetVacantUnitsAsync(propertyId);

        // Nur eigene filtern, wenn kein propertyId angegeben
        if (!propertyId.HasValue)
        {
            var ownerId = GetUserId();
            var properties = await _uow.Properties.GetByOwnerIdAsync(ownerId);
            var propertyIds = properties.Select(p => p.Id).ToHashSet();
            units = units.Where(u => propertyIds.Contains(u.PropertyId));
        }

        return Ok(units.Select(MapToDto));
    }

    [HttpPost]
    public async Task<ActionResult<UnitDto>> Create([FromBody] UnitCreateDto dto)
    {
        if (!await IsOwnerOfProperty(dto.PropertyId))
            return Forbid();

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

        _logger.LogInformation("Einheit {UnitName} erstellt für Property {PropertyId}", unit.Name, unit.PropertyId);
        return CreatedAtAction(nameof(GetById), new { id = unit.Id }, MapToDto(unit));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UnitUpdateDto dto)
    {
        var unit = await _uow.Units.GetByIdAsync(id);
        if (unit == null || !await IsOwnerOfUnit(unit))
            return NotFound();

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
        var unit = await _uow.Units.GetWithLeaseAsync(id);
        if (unit == null || !await IsOwnerOfUnit(unit))
            return NotFound();

        // Prüfen ob aktive Mietverträge existieren
        var activeLeases = unit.Leases?.Any(l => l.Status == LeaseStatus.Aktiv) ?? false;
        if (activeLeases)
            return BadRequest(new { message = "Einheit kann nicht gelöscht werden, da aktive Mietverträge existieren." });

        await _uow.Units.DeleteAsync(id);
        await _uow.SaveChangesAsync();

        _logger.LogInformation("Einheit {UnitId} gelöscht", id);
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
