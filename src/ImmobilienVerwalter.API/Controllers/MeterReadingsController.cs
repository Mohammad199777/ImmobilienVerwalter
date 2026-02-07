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
public class MeterReadingsController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<MeterReadingsController> _logger;

    public MeterReadingsController(IUnitOfWork uow, ILogger<MeterReadingsController> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private async Task<bool> IsOwnerOfUnit(Guid unitId)
    {
        var unit = await _uow.Units.GetByIdAsync(unitId);
        if (unit == null) return false;
        var property = await _uow.Properties.GetByIdAsync(unit.PropertyId);
        return property != null && property.OwnerId == GetUserId();
    }

    [HttpGet("unit/{unitId}")]
    public async Task<ActionResult<IEnumerable<MeterReadingDto>>> GetByUnit(Guid unitId)
    {
        if (!await IsOwnerOfUnit(unitId))
            return NotFound();

        var readings = await _uow.MeterReadings.GetByUnitIdAsync(unitId);
        return Ok(readings.Select(MapToDto));
    }

    [HttpPost]
    public async Task<ActionResult<MeterReadingDto>> Create([FromBody] MeterReadingCreateDto dto)
    {
        if (!await IsOwnerOfUnit(dto.UnitId))
            return Forbid();

        // Prüfen ob Zählerstand > vorheriger Stand
        var lastReading = await _uow.MeterReadings.GetLatestReadingAsync(dto.UnitId, dto.MeterType);
        if (lastReading != null && dto.Value < lastReading.Value)
            return BadRequest(new { message = $"Zählerstand ({dto.Value}) darf nicht kleiner als der vorherige Wert ({lastReading.Value}) sein." });

        var reading = new MeterReading
        {
            UnitId = dto.UnitId,
            MeterType = dto.MeterType,
            MeterNumber = dto.MeterNumber,
            Value = dto.Value,
            ReadingDate = dto.ReadingDate,
            Notes = dto.Notes,
            PhotoPath = dto.PhotoPath,
            PreviousValue = lastReading?.Value
        };

        await _uow.MeterReadings.AddAsync(reading);
        await _uow.SaveChangesAsync();

        _logger.LogInformation("Zählerstand für Unit {UnitId}, Typ {MeterType}: {Value}", dto.UnitId, dto.MeterType, dto.Value);
        return CreatedAtAction(nameof(GetById), new { id = reading.Id }, MapToDto(reading));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MeterReadingDto>> GetById(Guid id)
    {
        var reading = await _uow.MeterReadings.GetByIdAsync(id);
        if (reading == null) return NotFound();
        return Ok(MapToDto(reading));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] MeterReadingUpdateDto dto)
    {
        var reading = await _uow.MeterReadings.GetByIdAsync(id);
        if (reading == null) return NotFound();
        if (!await IsOwnerOfUnit(reading.UnitId))
            return NotFound();

        reading.MeterType = dto.MeterType;
        reading.MeterNumber = dto.MeterNumber;
        reading.Value = dto.Value;
        reading.ReadingDate = dto.ReadingDate;
        reading.Notes = dto.Notes;

        await _uow.MeterReadings.UpdateAsync(reading);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var reading = await _uow.MeterReadings.GetByIdAsync(id);
        if (reading == null) return NotFound();
        if (!await IsOwnerOfUnit(reading.UnitId))
            return NotFound();

        await _uow.MeterReadings.DeleteAsync(id);
        await _uow.SaveChangesAsync();

        _logger.LogInformation("Zählerstand {ReadingId} gelöscht", id);
        return NoContent();
    }

    private static MeterReadingDto MapToDto(MeterReading m) => new(
        m.Id, m.UnitId, m.Unit?.Name ?? "",
        m.MeterType, m.MeterNumber,
        m.Value, m.PreviousValue, m.Consumption,
        m.ReadingDate, m.Notes, m.CreatedAt);
}
