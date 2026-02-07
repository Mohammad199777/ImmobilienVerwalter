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
    string Name, string Street, string HouseNumber,
    string ZipCode, string City, string? Country,
    int? YearBuilt, decimal? TotalArea, int? NumberOfFloors,
    PropertyType Type, decimal? PurchasePrice, DateTime? PurchaseDate);

public record PropertyUpdateDto(
    string Name, string Street, string HouseNumber,
    string ZipCode, string City, string? Country,
    int? YearBuilt, decimal? TotalArea, int? NumberOfFloors,
    PropertyType Type, decimal? PurchasePrice, DateTime? PurchaseDate);

// ===== Unit DTOs =====
public record UnitDto(
    Guid Id, string Name, string? Description, int? Floor,
    decimal Area, int? Rooms, UnitType Type, UnitStatus Status,
    decimal TargetRent, Guid PropertyId, string PropertyName,
    TenantSummaryDto? CurrentTenant, DateTime CreatedAt);

public record UnitCreateDto(
    string Name, string? Description, int? Floor,
    decimal Area, int? Rooms, UnitType Type,
    decimal TargetRent, Guid PropertyId);

public record UnitUpdateDto(
    string Name, string? Description, int? Floor,
    decimal Area, int? Rooms, UnitType Type, UnitStatus Status,
    decimal TargetRent);

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
    string FirstName, string LastName, string Email,
    string? Phone, string? MobilePhone, string? PreviousAddress,
    string? Iban, string? Bic, string? BankName,
    DateTime? DateOfBirth, string? Occupation, decimal? MonthlyIncome,
    string? EmergencyContactName, string? EmergencyContactPhone,
    string? Notes);

public record TenantUpdateDto(
    string FirstName, string LastName, string Email,
    string? Phone, string? MobilePhone,
    string? Iban, string? Bic, string? BankName,
    DateTime? DateOfBirth, string? Occupation, decimal? MonthlyIncome,
    string? EmergencyContactName, string? EmergencyContactPhone,
    string? Notes);

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
    Guid TenantId, Guid UnitId,
    DateTime StartDate, DateTime? EndDate,
    decimal ColdRent, decimal AdditionalCosts,
    decimal DepositAmount, int NoticePeriodMonths,
    int PaymentDayOfMonth, string? Notes);

public record LeaseUpdateDto(
    decimal ColdRent, decimal AdditionalCosts,
    decimal DepositAmount, decimal DepositPaid,
    DepositStatus DepositStatus, LeaseStatus Status,
    DateTime? EndDate, DateTime? TerminationDate,
    DateTime? MoveOutDate, int NoticePeriodMonths,
    int PaymentDayOfMonth, string? Notes);

// ===== Payment DTOs =====
public record PaymentDto(
    Guid Id, Guid LeaseId, string TenantName,
    decimal Amount, DateTime PaymentDate, DateTime DueDate,
    int PaymentMonth, int PaymentYear,
    PaymentType Type, PaymentMethod Method, PaymentStatus Status,
    string? Reference, string? Notes, DateTime CreatedAt);

public record PaymentCreateDto(
    Guid LeaseId, decimal Amount, DateTime PaymentDate, DateTime DueDate,
    int PaymentMonth, int PaymentYear,
    PaymentType Type, PaymentMethod Method,
    PaymentStatus? Status,
    string? Reference, string? Notes, decimal? ExpectedAmount);

public record PaymentUpdateDto(
    decimal Amount, DateTime PaymentDate, DateTime DueDate,
    PaymentType Type, PaymentMethod Method, PaymentStatus Status,
    string? Reference, string? Notes);

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
    string Title, string? Description, decimal Amount,
    DateTime Date, DateTime? DueDate, ExpenseCategory Category,
    bool IsRecurring, RecurringInterval? RecurringInterval,
    bool IsAllocatable, bool IsTaxDeductible,
    string? Vendor, string? InvoiceNumber,
    Guid? PropertyId, Guid? UnitId, string? Notes);

// ===== MeterReading DTOs =====
public record MeterReadingDto(
    Guid Id, Guid UnitId, string UnitName,
    MeterType MeterType, string MeterNumber,
    decimal Value, decimal? PreviousValue, decimal? Consumption,
    DateTime ReadingDate, string? Notes, DateTime CreatedAt);

public record MeterReadingCreateDto(
    Guid UnitId, MeterType MeterType, string MeterNumber,
    decimal Value, DateTime ReadingDate, string? Notes,
    string? PhotoPath);

public record ExpenseUpdateDto(
    string Title, string? Description, decimal Amount,
    DateTime Date, DateTime? DueDate, ExpenseCategory Category,
    bool IsRecurring, RecurringInterval? RecurringInterval,
    bool IsAllocatable, bool IsTaxDeductible,
    string? Vendor, string? InvoiceNumber,
    Guid? PropertyId, Guid? UnitId, string? Notes);

public record MeterReadingUpdateDto(
    MeterType MeterType, string MeterNumber,
    decimal Value, DateTime ReadingDate, string? Notes);

// ===== Dashboard DTOs =====
public record DashboardDto(
    int TotalProperties, int TotalUnits, int OccupiedUnits, int VacantUnits,
    decimal OccupancyRate, decimal MonthlyIncome, decimal MonthlyExpenses,
    decimal MonthlyProfit, decimal YearlyIncome, decimal YearlyExpenses,
    int OverduePayments, int ExpiringLeases,
    IEnumerable<PaymentDto> RecentPayments,
    IEnumerable<LeaseDto> ExpiringLeasesList);

// ===== Auth DTOs =====
public record LoginDto(string Email, string Password);
public record RegisterDto(string Email, string Password, string FirstName, string LastName,
    string? Phone, string? Company);
public record AuthResponseDto(string Token, DateTime Expiration, UserDto User);
public record UserDto(Guid Id, string Email, string FirstName, string LastName,
    string FullName, string? Company, UserRole Role);
