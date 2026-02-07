namespace ImmobilienVerwalter.Core.Entities;

/// <summary>
/// Zählerstand – Wasser, Gas, Strom, Heizung
/// </summary>
public class MeterReading : BaseEntity
{
    public Guid UnitId { get; set; }
    public Unit Unit { get; set; } = null!;

    public MeterType MeterType { get; set; }
    public string MeterNumber { get; set; } = string.Empty; // Zählernummer
    public decimal Value { get; set; } // Zählerstand
    public DateTime ReadingDate { get; set; }

    public string? Notes { get; set; }
    public string? PhotoPath { get; set; } // Foto vom Zähler

    // Berechnung: Verbrauch seit letzter Ablesung
    public decimal? PreviousValue { get; set; }
    public decimal? Consumption => PreviousValue.HasValue ? Value - PreviousValue.Value : null;
}

public enum MeterType
{
    Wasser,
    WarmWasser,
    Gas,
    Strom,
    Heizung,
    Sonstige
}
