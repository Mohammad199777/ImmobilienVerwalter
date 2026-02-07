namespace ImmobilienVerwalter.Core.Entities;

/// <summary>
/// Dokument – Verträge, Belege, Protokolle, Korrespondenz
/// </summary>
public class Document : BaseEntity
{
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string StoragePath { get; set; } = string.Empty;

    public DocumentCategory Category { get; set; }
    public string? Description { get; set; }

    // Zuordnung (flexibel – kann zu verschiedenen Entities gehören)
    public Guid? PropertyId { get; set; }
    public Property? Property { get; set; }

    public Guid? UnitId { get; set; }
    public Unit? Unit { get; set; }

    public Guid? TenantId { get; set; }
    public Tenant? Tenant { get; set; }

    public Guid? LeaseId { get; set; }
    public Lease? Lease { get; set; }

    public Guid? ExpenseId { get; set; }
    public Expense? Expense { get; set; }

    // Hochgeladen von
    public Guid UploadedById { get; set; }
    public User UploadedBy { get; set; } = null!;
}

public enum DocumentCategory
{
    Mietvertrag,
    Uebergabeprotokoll,
    Nebenkostenabrechnung,
    Rechnung,
    Versicherungspolice,
    Grundbuchauszug,
    Energieausweis,
    Korrespondenz,
    Foto,
    Sonstige
}
