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
public class ExpensesController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<ExpensesController> _logger;

    public ExpensesController(IUnitOfWork uow, ILogger<ExpensesController> logger)
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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExpenseDto>>> GetAll()
    {
        var expenses = await _uow.Expenses.GetAllAsync();
        var propertyIds = await GetOwnerPropertyIdsAsync();
        // Nur Ausgaben mit eigenen Properties oder ohne Property-Zuordnung
        var filtered = expenses.Where(e =>
            e.PropertyId == null || propertyIds.Contains(e.PropertyId.Value));
        return Ok(filtered.Select(MapToDto));
    }

    [HttpGet("property/{propertyId}")]
    public async Task<ActionResult<IEnumerable<ExpenseDto>>> GetByProperty(Guid propertyId)
    {
        var propertyIds = await GetOwnerPropertyIdsAsync();
        if (!propertyIds.Contains(propertyId))
            return NotFound();

        var expenses = await _uow.Expenses.GetByPropertyIdAsync(propertyId);
        return Ok(expenses.Select(MapToDto));
    }

    [HttpGet("total/{year}")]
    public async Task<ActionResult<decimal>> GetTotal(int year, [FromQuery] int? month)
    {
        var total = await _uow.Expenses.GetTotalExpensesAsync(year, month);
        return Ok(total);
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseDto>> Create([FromBody] ExpenseCreateDto dto)
    {
        // Ownership prüfen wenn Property zugeordnet
        if (dto.PropertyId.HasValue)
        {
            var propertyIds = await GetOwnerPropertyIdsAsync();
            if (!propertyIds.Contains(dto.PropertyId.Value))
                return Forbid();
        }

        var expense = new Expense
        {
            Title = dto.Title,
            Description = dto.Description,
            Amount = dto.Amount,
            Date = dto.Date,
            DueDate = dto.DueDate,
            Category = dto.Category,
            IsRecurring = dto.IsRecurring,
            RecurringInterval = dto.RecurringInterval,
            IsAllocatable = dto.IsAllocatable,
            IsTaxDeductible = dto.IsTaxDeductible,
            Vendor = dto.Vendor,
            InvoiceNumber = dto.InvoiceNumber,
            PropertyId = dto.PropertyId,
            UnitId = dto.UnitId,
            Notes = dto.Notes
        };

        await _uow.Expenses.AddAsync(expense);
        await _uow.SaveChangesAsync();

        _logger.LogInformation("Ausgabe '{Title}' ({Amount}€) erstellt", dto.Title, dto.Amount);
        return CreatedAtAction(nameof(GetById), new { id = expense.Id }, MapToDto(expense));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExpenseDto>> GetById(Guid id)
    {
        var expense = await _uow.Expenses.GetByIdAsync(id);
        if (expense == null) return NotFound();
        return Ok(MapToDto(expense));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] ExpenseUpdateDto dto)
    {
        var expense = await _uow.Expenses.GetByIdAsync(id);
        if (expense == null) return NotFound();

        expense.Title = dto.Title;
        expense.Description = dto.Description;
        expense.Amount = dto.Amount;
        expense.Date = dto.Date;
        expense.DueDate = dto.DueDate;
        expense.Category = dto.Category;
        expense.IsRecurring = dto.IsRecurring;
        expense.RecurringInterval = dto.RecurringInterval;
        expense.IsAllocatable = dto.IsAllocatable;
        expense.IsTaxDeductible = dto.IsTaxDeductible;
        expense.Vendor = dto.Vendor;
        expense.InvoiceNumber = dto.InvoiceNumber;
        expense.PropertyId = dto.PropertyId;
        expense.UnitId = dto.UnitId;
        expense.Notes = dto.Notes;

        await _uow.Expenses.UpdateAsync(expense);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var expense = await _uow.Expenses.GetByIdAsync(id);
        if (expense == null) return NotFound();

        await _uow.Expenses.DeleteAsync(id);
        await _uow.SaveChangesAsync();

        _logger.LogInformation("Ausgabe {ExpenseId} gelöscht", id);
        return NoContent();
    }

    private static ExpenseDto MapToDto(Expense e) => new(
        e.Id, e.Title, e.Description, e.Amount,
        e.Date, e.DueDate, e.Category,
        e.IsRecurring, e.IsAllocatable, e.IsTaxDeductible,
        e.Vendor, e.InvoiceNumber,
        e.PropertyId, e.Property?.Name,
        e.UnitId, e.Unit?.Name,
        e.Notes, e.CreatedAt);
}
