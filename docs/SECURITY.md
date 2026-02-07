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

| Parameter       | Wert                     | Konfiguration                       |
| --------------- | ------------------------ | ----------------------------------- |
| **Algorithmus** | HMAC-SHA256              | Hardcoded                           |
| **Gültigkeit**  | 7 Tage                   | `AuthService.cs` → `AddDays(7)`     |
| **Issuer**      | `ImmobilienVerwalter`    | `appsettings.json` → `Jwt:Issuer`   |
| **Audience**    | `ImmobilienVerwalterApp` | `appsettings.json` → `Jwt:Audience` |
| **Secret**      | Min. 32 Zeichen          | `appsettings.json` → `Jwt:Secret`   |

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

Jede `Property` gehört einem Benutzer über `OwnerId`. Der `PropertiesController` filtert automatisch:

```csharp
private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

// Nur eigene Properties laden
var properties = await _uow.Properties.GetByOwnerIdAsync(GetUserId());
```

> ⚠️ **Achtung:** Diese Filterung ist aktuell nur im `PropertiesController` implementiert. Andere Controller (Units, Tenants, etc.) filtern **noch nicht** nach Eigentümer.

## CORS (Cross-Origin Resource Sharing)

```csharp
policy.WithOrigins("http://localhost:3000")  // Next.js Frontend
      .AllowAnyHeader()
      .AllowAnyMethod()
      .AllowCredentials();
```

| Parameter             | Wert                                |
| --------------------- | ----------------------------------- |
| **Erlaubte Origins**  | `http://localhost:3000`             |
| **Erlaubte Header**   | Alle                                |
| **Erlaubte Methoden** | Alle (GET, POST, PUT, DELETE, etc.) |
| **Credentials**       | Erlaubt (für Cookies/Auth-Header)   |

> 🔴 **TODO für Production:** CORS auf die tatsächliche Domain einschränken.

## Secrets-Management

### Development

Secrets liegen aktuell in `appsettings.json`:

```json
{
  "Jwt": {
    "Secret": "DeinSuperGeheimerSchluessel_MindestensSechs32Zeichen!!"
  }
}
```

### Production (empfohlen)

Secrets sollten NICHT in `appsettings.json` liegen, sondern über:

1. **User Secrets** (lokal):

   ```bash
   dotnet user-secrets set "Jwt:Secret" "EinSichererSchlüssel..."
   ```

2. **Umgebungsvariablen**:

   ```bash
   set Jwt__Secret=EinSichererSchlüssel...
   ```

3. **Azure Key Vault** (Cloud):
   ```csharp
   builder.Configuration.AddAzureKeyVault(...);
   ```

## Bekannte Sicherheits-Hinweise

| Priorität   | Thema                                                                | Status   |
| ----------- | -------------------------------------------------------------------- | -------- |
| 🔴 Hoch     | JWT Secret in `appsettings.json` → User Secrets / Env Vars verwenden | ⚠️ Offen |
| 🔴 Hoch     | Multi-Tenancy Filterung in allen Controllern durchsetzen             | ⚠️ Offen |
| 🟡 Mittel   | Rate Limiting für Login-Endpunkt                                     | ⚠️ Offen |
| 🟡 Mittel   | Refresh Token implementieren (statt 7-Tage Token)                    | ⚠️ Offen |
| 🟡 Mittel   | Input-Validierung (FluentValidation)                                 | ⚠️ Offen |
| 🟡 Mittel   | CORS für Production konfigurieren                                    | ⚠️ Offen |
| 🟢 Niedrig  | `[Authorize(Roles = "...")]` auf Controller-Ebene                    | ⚠️ Offen |
| ✅ Erledigt | Passwort-Hashing mit PBKDF2 + Salt                                   | ✅       |
| ✅ Erledigt | JWT-Authentifizierung                                                | ✅       |
| ✅ Erledigt | Soft Delete (keine physische Löschung)                               | ✅       |
| ✅ Erledigt | Timing-sichere Passwort-Prüfung                                      | ✅       |
