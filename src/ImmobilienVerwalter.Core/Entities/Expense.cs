namespace ImmobilienVerwalter.Core.Entities;

/// <summary>
/// Ausgaben / Kosten – Reparaturen, Versicherungen, Hausverwaltung etc.
/// </summary>
public class Expense : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public DateTime? DueDate { get; set; }

    public ExpenseCategory Category { get; set; }
    public bool IsRecurring { get; set; } = false;
    public RecurringInterval? RecurringInterval { get; set; }

    // Umlagefähig auf Mieter (Nebenkosten)?
    public bool IsAllocatable { get; set; } = false;

    // Zuordnung
    public Guid? PropertyId { get; set; }
    public Property? Property { get; set; }

    public Guid? UnitId { get; set; }
    public Unit? Unit { get; set; }

    // Dienstleister / Handwerker
    public string? Vendor { get; set; }
    public string? InvoiceNumber { get; set; }

    // Steuerlich relevant
    public bool IsTaxDeductible { get; set; } = true;

    public string? Notes { get; set; }

    // Navigation
    public ICollection<Document> Documents { get; set; } = new List<Document>();
}

public enum ExpenseCategory
{
    Reparatur,
    Wartung,
    Versicherung,
    Grundsteuer,
    Hausverwaltung,
    Wasser,
    Heizung,
    Strom,
    Muellabfuhr,
    Schornsteinfeger,
    Gartenpflege,
    Reinigung,
    Aufzug,
    Bankgebuehren,
    Zinsen,
    Abschreibung,
    Renovierung,
    Modernisierung,
    Rechtskosten,
    Sonstige
}

public enum RecurringInterval
{
    Monatlich,
    Quartalsweise,
    Halbjaehrlich,
    Jaehrlich
}
