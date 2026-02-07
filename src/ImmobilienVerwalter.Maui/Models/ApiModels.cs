namespace ImmobilienVerwalter.Maui.Models;

// Dashboard
public class DashboardData
{
    public int TotalProperties { get; set; }
    public int TotalUnits { get; set; }
    public int OccupiedUnits { get; set; }
    public int VacantUnits { get; set; }
    public decimal OccupancyRate { get; set; }
    public decimal MonthlyRentIncome { get; set; }
    public decimal MonthlyExpenses { get; set; }
    public decimal YearlyRentIncome { get; set; }
    public decimal YearlyExpenses { get; set; }
    public int OverduePayments { get; set; }
    public decimal OverdueAmount { get; set; }
    public int ExpiringLeases { get; set; }
}

// Property
public class PropertyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Street { get; set; } = "";
    public string HouseNumber { get; set; } = "";
    public string ZipCode { get; set; } = "";
    public string City { get; set; } = "";
    public int Type { get; set; }
    public int UnitCount { get; set; }
    public int OccupiedUnitCount { get; set; }
    public decimal TotalRentIncome { get; set; }
    public DateTime CreatedAt { get; set; }

    public string FullAddress => $"{Street} {HouseNumber}, {ZipCode} {City}";
}

// Unit
public class UnitDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public int? Floor { get; set; }
    public decimal Area { get; set; }
    public decimal? Rooms { get; set; }
    public int Type { get; set; }
    public int Status { get; set; }
    public decimal TargetRent { get; set; }
    public Guid PropertyId { get; set; }
    public string PropertyName { get; set; } = "";
    public TenantSummaryDto? CurrentTenant { get; set; }
    public DateTime CreatedAt { get; set; }

    public string StatusText => Status switch
    {
        0 => "Leer",
        1 => "Vermietet",
        2 => "Renovierung",
        3 => "Reserviert",
        _ => "Unbekannt"
    };
}

public class TenantSummaryDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
}

// Tenant
public class TenantDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Phone { get; set; }
    public string? MobilePhone { get; set; }
    public int ActiveLeases { get; set; }
    public DateTime CreatedAt { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}

// Payment
public class PaymentDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public DateTime DueDate { get; set; }
    public int PaymentMonth { get; set; }
    public int PaymentYear { get; set; }
    public int Type { get; set; }
    public int Status { get; set; }
    public string? TenantName { get; set; }
    public string? UnitName { get; set; }

    public string StatusText => Status switch
    {
        0 => "Ausstehend",
        1 => "Bezahlt",
        2 => "Überfällig",
        3 => "Teilweise bezahlt",
        _ => "Unbekannt"
    };
}
