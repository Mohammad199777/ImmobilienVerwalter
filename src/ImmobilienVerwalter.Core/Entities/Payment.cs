namespace ImmobilienVerwalter.Core.Entities;

/// <summary>
/// Mietzahlung – einzelne Zahlungseingänge
/// </summary>
public class Payment : BaseEntity
{
    public Guid LeaseId { get; set; }
    public Lease Lease { get; set; } = null!;

    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public DateTime DueDate { get; set; } // Fälligkeitsdatum

    // Welcher Monat wird bezahlt
    public int PaymentMonth { get; set; }
    public int PaymentYear { get; set; }

    public PaymentType Type { get; set; } = PaymentType.Miete;
    public PaymentMethod Method { get; set; } = PaymentMethod.Ueberweisung;
    public PaymentStatus Status { get; set; } = PaymentStatus.Eingegangen;

    public string? Reference { get; set; } // Verwendungszweck
    public string? Notes { get; set; }

    // Bei Teilzahlung: Fehlbetrag
    public decimal? ExpectedAmount { get; set; }
    public decimal Difference => (ExpectedAmount ?? 0) - Amount;
}

public enum PaymentType
{
    Miete,
    Kaution,
    Nachzahlung,
    Rueckzahlung,
    Sonstige
}

public enum PaymentMethod
{
    Ueberweisung,
    Lastschrift,
    Bar,
    PayPal,
    Sonstige
}

public enum PaymentStatus
{
    Eingegangen,
    Ausstehend,
    Ueberfaellig,
    Teilzahlung,
    Storniert
}
