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
    public MeterReadingsController(IUnitOfWork uow) => _uow = uow;

    [HttpGet("unit/{unitId}")]
    public async Task<ActionResult<IEnumerable<MeterReadingDto>>> GetByUnit(Guid unitId)
    {
        var readings = await _uow.MeterReadings.GetByUnitIdAsync(unitId);
        return Ok(readings.Select(MapToDto));
    }

    [HttpPost]
    public async Task<ActionResult<MeterReadingDto>> Create([FromBody] MeterReadingCreateDto dto)
    {
        // Letzten Zählerstand holen für Verbrauchsberechnung
        var lastReading = await _uow.MeterReadings.GetLatestReadingAsync(dto.UnitId, dto.MeterType);

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
        return Ok(MapToDto(reading));
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

        await _uow.MeterReadings.DeleteAsync(id);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    private static MeterReadingDto MapToDto(MeterReading m) => new(
        m.Id, m.UnitId, m.Unit?.Name ?? "",
        m.MeterType, m.MeterNumber,
        m.Value, m.PreviousValue, m.Consumption,
        m.ReadingDate, m.Notes, m.CreatedAt);
}
