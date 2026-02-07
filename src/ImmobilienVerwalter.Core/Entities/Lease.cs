namespace ImmobilienVerwalter.Core.Entities;

/// <summary>
/// Mietvertrag – Verbindung zwischen Mieter und Einheit
/// </summary>
public class Lease : BaseEntity
{
    // Vertragsparteien
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    public Guid UnitId { get; set; }
    public Unit Unit { get; set; } = null!;

    // Vertragsdaten
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; } // null = unbefristet
    public DateTime? TerminationDate { get; set; } // Kündigungsdatum
    public DateTime? MoveOutDate { get; set; } // tatsächliches Auszugsdatum

    // Kündigungsfrist in Monaten
    public int NoticePeriodMonths { get; set; } = 3;

    // Miete
    public decimal ColdRent { get; set; } // Kaltmiete
    public decimal AdditionalCosts { get; set; } // Nebenkosten-Vorauszahlung
    public decimal TotalRent => ColdRent + AdditionalCosts; // Warmmiete

    // Kaution
    public decimal DepositAmount { get; set; }
    public decimal DepositPaid { get; set; }
    public bool DepositFullyPaid => DepositPaid >= DepositAmount;
    public DepositStatus DepositStatus { get; set; } = DepositStatus.Ausstehend;

    // Mieterhöhung
    public DateTime? LastRentIncreaseDate { get; set; }

    // Zahlungstag (z.B. 1. oder 3. des Monats)
    public int PaymentDayOfMonth { get; set; } = 1;

    // Vertragsstatus
    public LeaseStatus Status { get; set; } = LeaseStatus.Aktiv;

    // Notizen
    public string? Notes { get; set; }

    // Navigation
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();

    public bool IsActive => Status == LeaseStatus.Aktiv && 
                           StartDate <= DateTime.UtcNow && 
                           (EndDate == null || EndDate > DateTime.UtcNow);
}

public enum LeaseStatus
{
    Aktiv,
    Gekuendigt,
    Beendet,
    Entwurf
}

public enum DepositStatus
{
    Ausstehend,
    Teilweise,
    Vollstaendig,
    Zurueckgezahlt
}
