namespace ImmobilienVerwalter.Core.Entities;

/// <summary>
/// Immobilie / Gebäude – das Hauptobjekt eines Vermieters
/// </summary>
public class Property : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    
    // Adresse
    public string Street { get; set; } = string.Empty;
    public string HouseNumber { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? Country { get; set; } = "Deutschland";

    // Gebäudedaten
    public int? YearBuilt { get; set; }
    public decimal? TotalArea { get; set; } // Gesamtfläche in m²
    public int? NumberOfFloors { get; set; }
    public PropertyType Type { get; set; } = PropertyType.Mehrfamilienhaus;

    // Kaufdaten
    public decimal? PurchasePrice { get; set; }
    public DateTime? PurchaseDate { get; set; }

    // Eigentümer
    public Guid OwnerId { get; set; }
    public User Owner { get; set; } = null!;

    // Navigation
    public ICollection<Unit> Units { get; set; } = new List<Unit>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    public string FullAddress => $"{Street} {HouseNumber}, {ZipCode} {City}";
}

public enum PropertyType
{
    Einfamilienhaus,
    Mehrfamilienhaus,
    Gewerbeimmobilie,
    MischGewerbeWohn,
    Garage,
    Grundstueck
}
