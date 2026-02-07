# 🔐 Sicherheit

> **Letzte Aktualisierung:** 2026-02-07  
> **Hinweis:** Bei Änderungen an Auth, Rollen, CORS oder Sicherheitskonfiguration dieses Dokument aktualisieren.

## Authentifizierung (JWT)

### Ablauf

```
1. Client sendet POST /api/auth/login mit { email, password }
2. Server prüft Passwort (PBKDF2-SHA256 + Salt)
3. Server erstellt JWT Token mit Claims
4. Client speichert Token
5. Client sendet Token bei jeder Anfrage: Authorization: Bearer <token>
6. Server validiert Token bei jedem Request (Middleware)
```

### Token-Konfiguration

| Parameter       | Wert                     | Konfiguration                                          |
| --------------- | ------------------------ | ------------------------------------------------------ |
| **Algorithmus** | HMAC-SHA256              | Hardcoded                                              |
| **Gültigkeit**  | 24 Stunden               | `AuthService.cs` → `AddHours(24)`                      |
| **Issuer**      | `ImmobilienVerwalter`    | `appsettings.json` → `Jwt:Issuer`                      |
| **Audience**    | `ImmobilienVerwalterApp` | `appsettings.json` → `Jwt:Audience`                    |
| **Secret**      | Min. 32 Zeichen          | Umgebungsvariable `JWT_SECRET` oder `appsettings.json` |
| **ClockSkew**   | 1 Minute                 | `Program.cs` → `TokenValidationParameters`             |

### Token-Claims

| Claim            | Inhalt                                     |
| ---------------- | ------------------------------------------ |
| `NameIdentifier` | User-ID (Guid)                             |
| `Email`          | E-Mail-Adresse                             |
| `Name`           | Vor- und Nachname                          |
| `Role`           | Benutzerrolle (Admin, Vermieter, Readonly) |

### Validierungsregeln

Der Server validiert bei jedem Request:

- ✅ Issuer stimmt überein
- ✅ Audience stimmt überein
- ✅ Token ist nicht abgelaufen
- ✅ Signatur ist gültig (Secret-Key)

## Passwort-Hashing

Passwörter werden **niemals im Klartext** gespeichert.

| Parameter          | Wert                                           |
| ------------------ | ---------------------------------------------- |
| **Algorithmus**    | PBKDF2 mit SHA-256                             |
| **Iterationen**    | 100.000                                        |
| **Salt**           | 16 Bytes (kryptografisch sicher, pro Benutzer) |
| **Hash-Länge**     | 32 Bytes                                       |
| **Speicherformat** | `{base64(salt)}.{base64(hash)}`                |
| **Vergleich**      | `FixedTimeEquals` (Timing-Attack-sicher)       |

## Benutzerrollen

| Rolle       | Beschreibung                   | Berechtigung           |
| ----------- | ------------------------------ | ---------------------- |
| `Admin`     | System-Administrator           | Voller Zugriff         |
| `Vermieter` | Standard-Benutzer (Eigentümer) | Eigene Daten verwalten |
| `Readonly`  | Nur-Lese-Zugriff               | Daten ansehen          |

> **Hinweis:** Die rollenbasierte Autorisierung ist aktuell über Claims vorbereitet, aber noch nicht auf Controller-Ebene mit `[Authorize(Roles = "...")]` durchgesetzt. Alle authentifizierten Benutzer haben aktuell gleichen Zugriff.

## Multi-Tenancy (Datentrennung)

Jede `Property` gehört einem Benutzer über `OwnerId`. **Alle Controller** filtern Daten nach dem angemeldeten Benutzer:

```csharp
private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

// Eigentümerprüfung in allen Controllern
var properties = await _uow.Properties.GetByOwnerIdAsync(GetUserId());
```

Controller mit Eigentümerprüfung (Ownership Checks):

- ✅ `PropertiesController` – filtert nach `OwnerId`
- ✅ `UnitsController` – prüft Property-Ownership
- ✅ `TenantsController` – filtert nach Leases/Units des Eigentümers
- ✅ `LeasesController` – prüft Unit/Property-Ownership
- ✅ `PaymentsController` – prüft Lease/Unit/Property-Ownership
- ✅ `ExpensesController` – prüft Property-Ownership
- ✅ `MeterReadingsController` – prüft Unit/Property-Ownership
- ✅ `DashboardController` – zeigt nur eigene Daten

## CORS (Cross-Origin Resource Sharing)

```csharp
// Konfigurierbar über appsettings.json
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:3000"];

policy.WithOrigins(allowedOrigins)
      .AllowAnyHeader()
      .AllowAnyMethod()
      .AllowCredentials();
```

| Parameter             | Wert                                                         |
| --------------------- | ------------------------------------------------------------ |
| **Erlaubte Origins**  | Konfigurierbar in `appsettings.json` → `Cors:AllowedOrigins` |
| **Standard-Origin**   | `http://localhost:3000`                                      |
| **Erlaubte Header**   | Alle                                                         |
| **Erlaubte Methoden** | Alle (GET, POST, PUT, DELETE, etc.)                          |
| **Credentials**       | Erlaubt (für Cookies/Auth-Header)                            |

> Für Production die `Cors:AllowedOrigins` in `appsettings.Production.json` auf die tatsächliche Domain setzen.

## Secrets-Management

### Development

In Development wird der JWT Secret aus `appsettings.Development.json` geladen:

```json
{
  "Jwt": {
    "Secret": "DevOnly_ImmobilienVerwalter_SuperSecretKey_Min32Chars!!"
  }
}
```

> ⚠️ Dieser Secret wird **nur** in Development genutzt und ist **nicht** in `appsettings.json` enthalten.

### Production

Der JWT Secret wird über die **Umgebungsvariable `JWT_SECRET`** gesetzt (bereits implementiert):

```bash
# Windows
set JWT_SECRET=EinSichererSchlüssel...

# Linux / macOS
export JWT_SECRET=EinSichererSchlüssel...
```

Alternativ:

1. **User Secrets** (lokal):

   ```bash
   dotnet user-secrets set "Jwt:Secret" "EinSichererSchlüssel..."
   ```

2. **Azure Key Vault** (Cloud):
   ```csharp
   builder.Configuration.AddAzureKeyVault(...);
   ```

> Die Reihenfolge der Secret-Auflösung: `JWT_SECRET` Env-Var → `appsettings.json` → Fehler (App startet nicht ohne Secret).

## Bekannte Sicherheits-Hinweise

| Priorität   | Thema                                                     | Status   |
| ----------- | --------------------------------------------------------- | -------- |
| 🟡 Mittel   | Rate Limiting für Login-Endpunkt                          | ⚠️ Offen |
| 🟡 Mittel   | Refresh Token implementieren (statt 24h Token)            | ⚠️ Offen |
| 🟢 Niedrig  | `[Authorize(Roles = "...")]` auf Controller-Ebene         | ⚠️ Offen |
| ✅ Erledigt | JWT Secret über Umgebungsvariable `JWT_SECRET`            | ✅       |
| ✅ Erledigt | Multi-Tenancy Filterung in allen Controllern              | ✅       |
| ✅ Erledigt | Input-Validierung mit DataAnnotations in allen DTOs       | ✅       |
| ✅ Erledigt | CORS konfigurierbar über `appsettings.json`               | ✅       |
| ✅ Erledigt | Token-Gültigkeit auf 24 Stunden reduziert (vorher 7 Tage) | ✅       |
| ✅ Erledigt | GlobalExceptionHandler Middleware                         | ✅       |
| ✅ Erledigt | Health Check Endpunkt (`/health`)                         | ✅       |
| ✅ Erledigt | Passwort-Hashing mit PBKDF2 + Salt                        | ✅       |
| ✅ Erledigt | JWT-Authentifizierung                                     | ✅       |
| ✅ Erledigt | Soft Delete (keine physische Löschung)                    | ✅       |
| ✅ Erledigt | Timing-sichere Passwort-Prüfung                           | ✅       |
