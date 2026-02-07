namespace ImmobilienVerwalter.Core.Entities;

/// <summary>
/// Benutzer – Vermieter / Admin der die App nutzt
/// </summary>
public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Company { get; set; } // Falls GmbH oder Hausverwaltung

    // Adresse des Vermieters
    public string? Street { get; set; }
    public string? ZipCode { get; set; }
    public string? City { get; set; }

    // Steuer
    public string? TaxId { get; set; } // Steuernummer

    public UserRole Role { get; set; } = UserRole.Vermieter;
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }

    // Navigation
    public ICollection<Property> Properties { get; set; } = new List<Property>();

    public string FullName => $"{FirstName} {LastName}";
}

public enum UserRole
{
    Admin,
    Vermieter,
    Readonly
}
