namespace ImmobilienVerwalter.Core.Entities;

/// <summary>
/// Einheit / Wohnung / Gewerbeeinheit innerhalb einer Immobilie
/// </summary>
public class Unit : BaseEntity
{
    public string Name { get; set; } = string.Empty; // z.B. "Wohnung 1 OG links"
    public string? Description { get; set; }

    public int? Floor { get; set; } // Stockwerk
    public decimal Area { get; set; } // Wohnfläche in m²
    public int? Rooms { get; set; } // Anzahl Zimmer
    public UnitType Type { get; set; } = UnitType.Wohnung;
    public UnitStatus Status { get; set; } = UnitStatus.Leer;

    // Kaltmiete die angesetzt werden soll
    public decimal TargetRent { get; set; }

    // Zugehörige Immobilie
    public Guid PropertyId { get; set; }
    public Property Property { get; set; } = null!;

    // Navigation
    public ICollection<Lease> Leases { get; set; } = new List<Lease>();
    public ICollection<MeterReading> MeterReadings { get; set; } = new List<MeterReading>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();

    // Aktueller Mieter (abgeleitet aus aktivem Mietvertrag)
    public Lease? ActiveLease => Leases?.FirstOrDefault(l => l.IsActive);
}

public enum UnitType
{
    Wohnung,
    Gewerbe,
    Garage,
    Stellplatz,
    Keller,
    Sonstige
}

public enum UnitStatus
{
    Vermietet,
    Leer,
    InRenovierung,
    Eigennutzung
}
