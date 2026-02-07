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
public class PropertiesController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<PropertiesController> _logger;

    public PropertiesController(IUnitOfWork uow, ILogger<PropertiesController> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PropertyDto>>> GetAll()
    {
        var properties = await _uow.Properties.GetByOwnerIdAsync(GetUserId());
        var dtos = properties.Select(p => MapToDto(p));
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PropertyDto>> GetById(Guid id)
    {
        var property = await _uow.Properties.GetWithUnitsAsync(id);
        if (property == null || property.OwnerId != GetUserId())
            return NotFound();
        return Ok(MapToDto(property));
    }

    [HttpPost]
    public async Task<ActionResult<PropertyDto>> Create([FromBody] PropertyCreateDto dto)
    {
        var property = new Property
        {
            Name = dto.Name,
            Street = dto.Street,
            HouseNumber = dto.HouseNumber,
            ZipCode = dto.ZipCode,
            City = dto.City,
            Country = dto.Country,
            YearBuilt = dto.YearBuilt,
            TotalArea = dto.TotalArea,
            NumberOfFloors = dto.NumberOfFloors,
            Type = dto.Type,
            PurchasePrice = dto.PurchasePrice,
            PurchaseDate = dto.PurchaseDate,
            OwnerId = GetUserId()
        };

        await _uow.Properties.AddAsync(property);
        await _uow.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = property.Id }, MapToDto(property));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] PropertyUpdateDto dto)
    {
        var property = await _uow.Properties.GetByIdAsync(id);
        if (property == null || property.OwnerId != GetUserId())
            return NotFound();

        property.Name = dto.Name;
        property.Street = dto.Street;
        property.HouseNumber = dto.HouseNumber;
        property.ZipCode = dto.ZipCode;
        property.City = dto.City;
        property.Country = dto.Country;
        property.YearBuilt = dto.YearBuilt;
        property.TotalArea = dto.TotalArea;
        property.NumberOfFloors = dto.NumberOfFloors;
        property.Type = dto.Type;
        property.PurchasePrice = dto.PurchasePrice;
        property.PurchaseDate = dto.PurchaseDate;

        await _uow.Properties.UpdateAsync(property);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var property = await _uow.Properties.GetWithUnitsAsync(id);
        if (property == null || property.OwnerId != GetUserId())
            return NotFound();

        if (property.Units != null && property.Units.Any())
            return BadRequest(new { message = "Immobilie kann nicht gelöscht werden, da noch Einheiten zugeordnet sind." });

        await _uow.Properties.DeleteAsync(id);
        await _uow.SaveChangesAsync();

        _logger.LogInformation("Immobilie {PropertyId} gelöscht", id);
        return NoContent();
    }

    private static PropertyDto MapToDto(Property p) => new(
        p.Id, p.Name, p.Street, p.HouseNumber,
        p.ZipCode, p.City, p.Country,
        p.YearBuilt, p.TotalArea, p.NumberOfFloors,
        p.Type, p.PurchasePrice, p.PurchaseDate,
        p.FullAddress,
        p.Units?.Count ?? 0,
        p.Units?.Count(u => u.Status == UnitStatus.Vermietet) ?? 0,
        p.CreatedAt);
}
