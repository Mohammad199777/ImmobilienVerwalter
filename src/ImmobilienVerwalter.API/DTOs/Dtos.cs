using System.ComponentModel.DataAnnotations;
using ImmobilienVerwalter.Core.Entities;

namespace ImmobilienVerwalter.API.DTOs;

// ===== Property DTOs =====
public record PropertyDto(
    Guid Id, string Name, string Street, string HouseNumber,
    string ZipCode, string City, string? Country,
    int? YearBuilt, decimal? TotalArea, int? NumberOfFloors,
    PropertyType Type, decimal? PurchasePrice, DateTime? PurchaseDate,
    string FullAddress, int UnitCount, int OccupiedUnits,
    DateTime CreatedAt);

public record PropertyCreateDto(
    [Required(ErrorMessage = "Name ist erforderlich.")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Name muss zwischen 2 und 200 Zeichen lang sein.")]
    string Name,

    [Required(ErrorMessage = "Straße ist erforderlich.")]
    [StringLength(200, ErrorMessage = "Straße darf maximal 200 Zeichen lang sein.")]
    string Street,

    [Required(ErrorMessage = "Hausnummer ist erforderlich.")]
    [StringLength(20, ErrorMessage = "Hausnummer darf maximal 20 Zeichen lang sein.")]
    string HouseNumber,

    [Required(ErrorMessage = "PLZ ist erforderlich.")]
    [RegularExpression(@"^\d{5}$", ErrorMessage = "PLZ muss 5 Ziffern enthalten.")]
    string ZipCode,

    [Required(ErrorMessage = "Stadt ist erforderlich.")]
    [StringLength(100, ErrorMessage = "Stadt darf maximal 100 Zeichen lang sein.")]
    string City,

    [StringLength(50)]
    string? Country,

    [Range(1800, 2100, ErrorMessage = "Baujahr muss zwischen 1800 und 2100 liegen.")]
    int? YearBuilt,

    [Range(0.01, 999999, ErrorMessage = "Fläche muss positiv sein.")]
    decimal? TotalArea,

    [Range(1, 200, ErrorMessage = "Stockwerke müssen zwischen 1 und 200 liegen.")]
    int? NumberOfFloors,

    PropertyType Type,

    [Range(0, 999999999, ErrorMessage = "Kaufpreis muss positiv sein.")]
    decimal? PurchasePrice,

    DateTime? PurchaseDate);

public record PropertyUpdateDto(
    [Required(ErrorMessage = "Name ist erforderlich.")]
    [StringLength(200, MinimumLength = 2)]
    string Name,

    [Required][StringLength(200)] string Street,
    [Required][StringLength(20)] string HouseNumber,
    [Required][RegularExpression(@"^\d{5}$", ErrorMessage = "PLZ muss 5 Ziffern enthalten.")] string ZipCode,
    [Required][StringLength(100)] string City,
    [StringLength(50)] string? Country,
    [Range(1800, 2100)] int? YearBuilt,
    [Range(0.01, 999999)] decimal? TotalArea,
    [Range(1, 200)] int? NumberOfFloors,
    PropertyType Type,
    [Range(0, 999999999)] decimal? PurchasePrice,
    DateTime? PurchaseDate);

// ===== Unit DTOs =====
public record UnitDto(
    Guid Id, string Name, string? Description, int? Floor,
    decimal Area, int? Rooms, UnitType Type, UnitStatus Status,
    decimal TargetRent, Guid PropertyId, string PropertyName,
    TenantSummaryDto? CurrentTenant, DateTime CreatedAt);

public record UnitCreateDto(
    [Required(ErrorMessage = "Name ist erforderlich.")]
    [StringLength(200, MinimumLength = 1)]
    string Name,

    [StringLength(500)] string? Description,
    [Range(-5, 100, ErrorMessage = "Etage muss zwischen -5 und 100 liegen.")] int? Floor,

    [Required][Range(0.01, 99999, ErrorMessage = "Fläche muss positiv sein.")]
    decimal Area,

    [Range(1, 50, ErrorMessage = "Zimmeranzahl muss zwischen 1 und 50 liegen.")] int? Rooms,
    UnitType Type,

    [Required][Range(0, 99999, ErrorMessage = "Soll-Miete muss positiv sein.")]
    decimal TargetRent,

    [Required(ErrorMessage = "Immobilie muss zugeordnet werden.")]
    Guid PropertyId);

public record UnitUpdateDto(
    [Required][StringLength(200, MinimumLength = 1)] string Name,
    [StringLength(500)] string? Description,
    [Range(-5, 100)] int? Floor,
    [Required][Range(0.01, 99999)] decimal Area,
    [Range(1, 50)] int? Rooms,
    UnitType Type,
    UnitStatus Status,
    [Required][Range(0, 99999)] decimal TargetRent);

// ===== Tenant DTOs =====
public record TenantDto(
    Guid Id, string FirstName, string LastName, string FullName,
    string Email, string? Phone, string? MobilePhone,
    string? PreviousAddress, string? Iban, string? Bic, string? BankName,
    DateTime? DateOfBirth, string? Occupation, decimal? MonthlyIncome,
    string? EmergencyContactName, string? EmergencyContactPhone,
    string? Notes, int ActiveLeases, DateTime CreatedAt);

public record TenantSummaryDto(Guid Id, string FullName, string Email);

public record TenantCreateDto(
    [Required(ErrorMessage = "Vorname ist erforderlich.")]
    [StringLength(100, MinimumLength = 1)]
    string FirstName,

    [Required(ErrorMessage = "Nachname ist erforderlich.")]
    [StringLength(100, MinimumLength = 1)]
    string LastName,

    [Required(ErrorMessage = "E-Mail ist erforderlich.")]
    [EmailAddress(ErrorMessage = "Ungültige E-Mail-Adresse.")]
    [StringLength(256)]
    string Email,

    [Phone(ErrorMessage = "Ungültige Telefonnummer.")][StringLength(30)] string? Phone,
    [Phone][StringLength(30)] string? MobilePhone,
    [StringLength(500)] string? PreviousAddress,
    [StringLength(34)] string? Iban,
    [StringLength(11)] string? Bic,
    [StringLength(100)] string? BankName,
    DateTime? DateOfBirth,
    [StringLength(100)] string? Occupation,
    [Range(0, 999999, ErrorMessage = "Einkommen muss positiv sein.")] decimal? MonthlyIncome,
    [StringLength(200)] string? EmergencyContactName,
    [Phone][StringLength(30)] string? EmergencyContactPhone,
    [StringLength(2000)] string? Notes);

public record TenantUpdateDto(
    [Required][StringLength(100, MinimumLength = 1)] string FirstName,
    [Required][StringLength(100, MinimumLength = 1)] string LastName,
    [Required][EmailAddress][StringLength(256)] string Email,
    [Phone][StringLength(30)] string? Phone,
    [Phone][StringLength(30)] string? MobilePhone,
    [StringLength(34)] string? Iban,
    [StringLength(11)] string? Bic,
    [StringLength(100)] string? BankName,
    DateTime? DateOfBirth,
    [StringLength(100)] string? Occupation,
    [Range(0, 999999)] decimal? MonthlyIncome,
    [StringLength(200)] string? EmergencyContactName,
    [Phone][StringLength(30)] string? EmergencyContactPhone,
    [StringLength(2000)] string? Notes);

// ===== Lease DTOs =====
public record LeaseDto(
    Guid Id, Guid TenantId, string TenantName,
    Guid UnitId, string UnitName, string PropertyName,
    DateTime StartDate, DateTime? EndDate,
    decimal ColdRent, decimal AdditionalCosts, decimal TotalRent,
    decimal DepositAmount, decimal DepositPaid, DepositStatus DepositStatus,
    LeaseStatus Status, int NoticePeriodMonths, int PaymentDayOfMonth,
    bool IsActive, string? Notes, DateTime CreatedAt);

public record LeaseCreateDto(
    [Required(ErrorMessage = "Mieter muss ausgewählt werden.")]
    Guid TenantId,

    [Required(ErrorMessage = "Einheit muss ausgewählt werden.")]
    Guid UnitId,

    [Required] DateTime StartDate,
    DateTime? EndDate,

    [Required][Range(0.01, 99999, ErrorMessage = "Kaltmiete muss positiv sein.")]
    decimal ColdRent,

    [Required][Range(0, 99999, ErrorMessage = "Nebenkosten müssen positiv sein.")]
    decimal AdditionalCosts,

    [Required][Range(0, 99999, ErrorMessage = "Kaution muss positiv sein.")]
    decimal DepositAmount,

    [Range(0, 24, ErrorMessage = "Kündigungsfrist muss zwischen 0 und 24 Monaten liegen.")]
    int NoticePeriodMonths,

    [Range(1, 28, ErrorMessage = "Zahltag muss zwischen 1 und 28 liegen.")]
    int PaymentDayOfMonth,

    [StringLength(2000)] string? Notes) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate.HasValue && EndDate <= StartDate)
            yield return new ValidationResult("Enddatum muss nach dem Startdatum liegen.", [nameof(EndDate)]);
        if (DepositAmount > ColdRent * 3)
            yield return new ValidationResult("Kaution darf maximal 3 Kaltmieten betragen (§551 BGB).", [nameof(DepositAmount)]);
    }
}

public record LeaseUpdateDto(
    [Required][Range(0.01, 99999)] decimal ColdRent,
    [Required][Range(0, 99999)] decimal AdditionalCosts,
    [Required][Range(0, 99999)] decimal DepositAmount,
    [Range(0, 99999)] decimal DepositPaid,
    DepositStatus DepositStatus,
    LeaseStatus Status,
    DateTime? EndDate,
    DateTime? TerminationDate,
    DateTime? MoveOutDate,
    [Range(0, 24)] int NoticePeriodMonths,
    [Range(1, 28)] int PaymentDayOfMonth,
    [StringLength(2000)] string? Notes);

// ===== Payment DTOs =====
public record PaymentDto(
    Guid Id, Guid LeaseId, string TenantName,
    decimal Amount, DateTime PaymentDate, DateTime DueDate,
    int PaymentMonth, int PaymentYear,
    PaymentType Type, PaymentMethod Method, PaymentStatus Status,
    string? Reference, string? Notes, DateTime CreatedAt);

public record PaymentCreateDto(
    [Required(ErrorMessage = "Mietvertrag muss ausgewählt werden.")]
    Guid LeaseId,

    [Required][Range(0.01, 999999, ErrorMessage = "Betrag muss positiv sein.")]
    decimal Amount,

    [Required] DateTime PaymentDate,
    [Required] DateTime DueDate,

    [Range(1, 12, ErrorMessage = "Monat muss zwischen 1 und 12 liegen.")]
    int PaymentMonth,

    [Range(2000, 2100, ErrorMessage = "Jahr muss zwischen 2000 und 2100 liegen.")]
    int PaymentYear,

    PaymentType Type,
    PaymentMethod Method,
    PaymentStatus? Status,
    [StringLength(500)] string? Reference,
    [StringLength(2000)] string? Notes,
    [Range(0, 999999)] decimal? ExpectedAmount);

public record PaymentUpdateDto(
    [Required][Range(0.01, 999999)] decimal Amount,
    [Required] DateTime PaymentDate,
    [Required] DateTime DueDate,
    PaymentType Type,
    PaymentMethod Method,
    PaymentStatus Status,
    [StringLength(500)] string? Reference,
    [StringLength(2000)] string? Notes);

// ===== Expense DTOs =====
public record ExpenseDto(
    Guid Id, string Title, string? Description, decimal Amount,
    DateTime Date, DateTime? DueDate, ExpenseCategory Category,
    bool IsRecurring, bool IsAllocatable, bool IsTaxDeductible,
    string? Vendor, string? InvoiceNumber,
    Guid? PropertyId, string? PropertyName,
    Guid? UnitId, string? UnitName,
    string? Notes, DateTime CreatedAt);

public record ExpenseCreateDto(
    [Required(ErrorMessage = "Titel ist erforderlich.")]
    [StringLength(300, MinimumLength = 2)]
    string Title,

    [StringLength(1000)] string? Description,

    [Required][Range(0.01, 99999999, ErrorMessage = "Betrag muss positiv sein.")]
    decimal Amount,

    [Required] DateTime Date,
    DateTime? DueDate,
    ExpenseCategory Category,
    bool IsRecurring,
    RecurringInterval? RecurringInterval,
    bool IsAllocatable,
    bool IsTaxDeductible,
    [StringLength(200)] string? Vendor,
    [StringLength(100)] string? InvoiceNumber,
    Guid? PropertyId,
    Guid? UnitId,
    [StringLength(2000)] string? Notes);

// ===== MeterReading DTOs =====
public record MeterReadingDto(
    Guid Id, Guid UnitId, string UnitName,
    MeterType MeterType, string MeterNumber,
    decimal Value, decimal? PreviousValue, decimal? Consumption,
    DateTime ReadingDate, string? Notes, DateTime CreatedAt);

public record MeterReadingCreateDto(
    [Required(ErrorMessage = "Einheit muss ausgewählt werden.")]
    Guid UnitId,

    MeterType MeterType,

    [Required(ErrorMessage = "Zählernummer ist erforderlich.")]
    [StringLength(50)]
    string MeterNumber,

    [Required][Range(0, 9999999, ErrorMessage = "Zählerstand muss positiv sein.")]
    decimal Value,

    [Required] DateTime ReadingDate,
    [StringLength(1000)] string? Notes,
    [StringLength(500)] string? PhotoPath);

public record ExpenseUpdateDto(
    [Required][StringLength(300, MinimumLength = 2)] string Title,
    [StringLength(1000)] string? Description,
    [Required][Range(0.01, 99999999)] decimal Amount,
    [Required] DateTime Date,
    DateTime? DueDate,
    ExpenseCategory Category,
    bool IsRecurring,
    RecurringInterval? RecurringInterval,
    bool IsAllocatable,
    bool IsTaxDeductible,
    [StringLength(200)] string? Vendor,
    [StringLength(100)] string? InvoiceNumber,
    Guid? PropertyId,
    Guid? UnitId,
    [StringLength(2000)] string? Notes);

public record MeterReadingUpdateDto(
    MeterType MeterType,
    [Required][StringLength(50)] string MeterNumber,
    [Required][Range(0, 9999999)] decimal Value,
    [Required] DateTime ReadingDate,
    [StringLength(1000)] string? Notes);

// ===== Dashboard DTOs =====
public record DashboardDto(
    int TotalProperties, int TotalUnits, int OccupiedUnits, int VacantUnits,
    decimal OccupancyRate, decimal MonthlyIncome, decimal MonthlyExpenses,
    decimal MonthlyProfit, decimal YearlyIncome, decimal YearlyExpenses,
    int OverduePayments, int ExpiringLeases,
    IEnumerable<PaymentDto> RecentPayments,
    IEnumerable<LeaseDto> ExpiringLeasesList);

// ===== Auth DTOs =====
public record LoginDto(
    [Required(ErrorMessage = "E-Mail ist erforderlich.")]
    [EmailAddress(ErrorMessage = "Ungültige E-Mail-Adresse.")]
    string Email,

    [Required(ErrorMessage = "Passwort ist erforderlich.")]
    string Password);

public record RegisterDto(
    [Required(ErrorMessage = "E-Mail ist erforderlich.")]
    [EmailAddress(ErrorMessage = "Ungültige E-Mail-Adresse.")]
    [StringLength(256)]
    string Email,

    [Required(ErrorMessage = "Passwort ist erforderlich.")]
    [MinLength(8, ErrorMessage = "Passwort muss mindestens 8 Zeichen lang sein.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$",
        ErrorMessage = "Passwort muss Groß-/Kleinbuchstaben, eine Zahl und ein Sonderzeichen enthalten.")]
    string Password,

    [Required(ErrorMessage = "Vorname ist erforderlich.")]
    [StringLength(100, MinimumLength = 1)]
    string FirstName,

    [Required(ErrorMessage = "Nachname ist erforderlich.")]
    [StringLength(100, MinimumLength = 1)]
    string LastName,

    [Phone][StringLength(30)] string? Phone,
    [StringLength(200)] string? Company);

public record AuthResponseDto(string Token, DateTime Expiration, UserDto User);
public record UserDto(Guid Id, string Email, string FirstName, string LastName,
    string FullName, string? Company, UserRole Role);
