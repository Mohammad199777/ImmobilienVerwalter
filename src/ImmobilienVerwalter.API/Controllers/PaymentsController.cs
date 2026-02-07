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
public class PaymentsController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IUnitOfWork uow, ILogger<PaymentsController> logger)
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

    private bool IsPaymentOwnedByUser(Payment payment, HashSet<Guid> propertyIds)
    {
        return payment.Lease?.Unit?.PropertyId != null
               && propertyIds.Contains(payment.Lease.Unit.PropertyId);
    }

    [HttpGet("lease/{leaseId}")]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetByLease(Guid leaseId)
    {
        var lease = await _uow.Leases.GetByIdAsync(leaseId);
        if (lease == null) return NotFound();

        var payments = await _uow.Payments.GetByLeaseIdAsync(leaseId);
        return Ok(payments.Select(MapToDto));
    }

    [HttpGet("overdue")]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetOverdue()
    {
        var payments = await _uow.Payments.GetOverduePaymentsAsync();
        var propertyIds = await GetOwnerPropertyIdsAsync();
        var filtered = payments.Where(p => IsPaymentOwnedByUser(p, propertyIds));
        return Ok(filtered.Select(MapToDto));
    }

    [HttpGet("month/{year}/{month}")]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetByMonth(int year, int month)
    {
        if (month < 1 || month > 12 || year < 2000 || year > 2100)
            return BadRequest(new { message = "Ungültiger Monat oder Jahr." });

        var payments = await _uow.Payments.GetByMonthAsync(year, month);
        var propertyIds = await GetOwnerPropertyIdsAsync();
        var filtered = payments.Where(p => IsPaymentOwnedByUser(p, propertyIds));
        return Ok(filtered.Select(MapToDto));
    }

    [HttpGet("income/{year}")]
    public async Task<ActionResult<decimal>> GetYearlyIncome(int year, [FromQuery] int? month)
    {
        // TODO: Owner-spezifisch filtern
        var income = await _uow.Payments.GetTotalIncomeAsync(year, month);
        return Ok(income);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PaymentDto>> GetById(Guid id)
    {
        var payment = await _uow.Payments.GetByIdAsync(id);
        if (payment == null) return NotFound();
        return Ok(MapToDto(payment));
    }

    [HttpPost]
    public async Task<ActionResult<PaymentDto>> Create([FromBody] PaymentCreateDto dto)
    {
        // Prüfen ob Lease existiert
        var lease = await _uow.Leases.GetByIdAsync(dto.LeaseId);
        if (lease == null)
            return BadRequest(new { message = "Mietvertrag nicht gefunden." });

        var payment = new Payment
        {
            LeaseId = dto.LeaseId,
            Amount = dto.Amount,
            PaymentDate = dto.PaymentDate,
            DueDate = dto.DueDate,
            PaymentMonth = dto.PaymentMonth,
            PaymentYear = dto.PaymentYear,
            Type = dto.Type,
            Method = dto.Method,
            Reference = dto.Reference,
            Notes = dto.Notes,
            ExpectedAmount = dto.ExpectedAmount ?? lease.TotalRent,
            Status = dto.Status ?? PaymentStatus.Eingegangen
        };

        await _uow.Payments.AddAsync(payment);
        await _uow.SaveChangesAsync();

        _logger.LogInformation("Zahlung {Amount}€ für Lease {LeaseId} erfasst", dto.Amount, dto.LeaseId);
        return CreatedAtAction(nameof(GetById), new { id = payment.Id }, MapToDto(payment));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] PaymentUpdateDto dto)
    {
        var payment = await _uow.Payments.GetByIdAsync(id);
        if (payment == null) return NotFound();

        payment.Amount = dto.Amount;
        payment.PaymentDate = dto.PaymentDate;
        payment.DueDate = dto.DueDate;
        payment.Type = dto.Type;
        payment.Method = dto.Method;
        payment.Status = dto.Status;
        payment.Reference = dto.Reference;
        payment.Notes = dto.Notes;

        await _uow.Payments.UpdateAsync(payment);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var payment = await _uow.Payments.GetByIdAsync(id);
        if (payment == null) return NotFound();

        await _uow.Payments.DeleteAsync(id);
        await _uow.SaveChangesAsync();

        _logger.LogInformation("Zahlung {PaymentId} gelöscht", id);
        return NoContent();
    }

    private static PaymentDto MapToDto(Payment p) => new(
        p.Id, p.LeaseId, p.Lease?.Tenant?.FullName ?? "",
        p.Amount, p.PaymentDate, p.DueDate,
        p.PaymentMonth, p.PaymentYear,
        p.Type, p.Method, p.Status,
        p.Reference, p.Notes, p.CreatedAt);
}
