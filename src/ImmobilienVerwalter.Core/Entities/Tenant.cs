namespace ImmobilienVerwalter.Core.Entities;

/// <summary>
/// Mieter – Personen die eine Einheit mieten
/// </summary>
public class Tenant : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? MobilePhone { get; set; }

    // Vorherige Adresse (vor Einzug)
    public string? PreviousAddress { get; set; }

    // Bankverbindung für SEPA
    public string? Iban { get; set; }
    public string? Bic { get; set; }
    public string? BankName { get; set; }

    // Persönliche Daten
    public DateTime? DateOfBirth { get; set; }
    public string? Occupation { get; set; } // Beruf
    public decimal? MonthlyIncome { get; set; } // Monatliches Einkommen

    // Notfallkontakt
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }

    // Notizen
    public string? Notes { get; set; }

    // Navigation
    public ICollection<Lease> Leases { get; set; } = new List<Lease>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();

    public string FullName => $"{FirstName} {LastName}";
}
