# 📝 Changelog

Alle nennenswerten Änderungen am Projekt werden in dieser Datei dokumentiert.

Das Format basiert auf [Keep a Changelog](https://keepachangelog.com/de/1.0.0/)
und dieses Projekt folgt [Semantic Versioning](https://semver.org/lang/de/).

> **Hinweis:** Bei jedem Release oder bedeutsamen Feature einen neuen Eintrag hinzufügen.

---

## [0.2.0] – 2026-02-07

### 🔒 Sicherheit & Hardening

**Backend**

- JWT Secret wird über Umgebungsvariable `JWT_SECRET` geladen (nicht mehr in `appsettings.json`)
- Token-Gültigkeit von 7 Tagen auf 24 Stunden reduziert
- ClockSkew auf 1 Minute begrenzt
- Ownership-Checks in allen Controllern (Multi-Tenancy vollständig durchgesetzt)
- Strukturiertes Logging mit `ILogger<T>` in allen Controllern
- Business-Logik-Guards (z.B. Duplikat-Prüfungen, Status-Validierung)
- `GlobalExceptionHandler` Middleware für zentrale Fehlerbehandlung
- Health Check Endpunkt `/health` mit DB-Verbindungsprüfung
- HSTS in Production aktiviert
- CORS konfigurierbar über `appsettings.json` → `Cors:AllowedOrigins`

**DTOs**

- Umfassende Validierung mit DataAnnotations in allen DTOs
- Kautionslimit nach §551 BGB (max. 3 × Kaltmiete)
- E-Mail-, IBAN-, PLZ-Format-Validierung
- Pflichtfeld- und Bereichsprüfungen

### Geändert

**Datenbank**

- Umstellung von SQL Server (LocalDB) auf **SQLite** (plattformunabhängig, keine externe DB nötig)
- Datenbankdatei: `ImmobilienVerwalter.db` (wird automatisch erstellt)
- EF Core Migrations statt `EnsureCreated()`
- Initiale Migration `InitialCreate` erstellt

**Backend**

- Alle 7 CRUD-Controller komplett überarbeitet (Properties, Units, Tenants, Leases, Payments, Expenses, MeterReadings)
- `DashboardService` filtert nun nach Eigentümer (Owner-basierte Aggregation)
- `AuthService` nutzt 24h Token-Gültigkeit
- `Program.cs` gehärtet: try-catch Migration mit EnsureCreated-Fallback
- AutoMapper entfernt (manuelles Mapping in Controllern)

**Web-Frontend (Next.js)**

- Toast-Benachrichtigungssystem (`useToast` Hook + `ToastContainer` Komponente)
- Next.js Auth-Middleware für geschützte Routen
- Alle 7 Dashboard-Seiten komplett überarbeitet:
  - Immobilien, Einheiten, Mieter, Mietverträge, Zahlungen, Ausgaben, Zählerstände
- Ladeanimationen und Fehlermeldungen via Toast
- Barrierefreiheit: ARIA-Labels, semantisches HTML, `lang="de"`
- Root-Layout mit vollständigen Metadaten

**Dokumentation**

- Alle Docs aktualisiert (Port 5013, SQLite, JWT-Env-Var, Multi-Tenancy, Validierung)
- SECURITY.md: Bekannte-Hinweise-Tabelle aktualisiert (6 neue ✅ Erledigt-Einträge)

### Entfernt

- `AutoMapper.Extensions.Microsoft.DependencyInjection` NuGet-Paket
- `Microsoft.EntityFrameworkCore.SqlServer` NuGet-Paket (ersetzt durch Sqlite)
- SQL Server LocalDB als Voraussetzung

---

## [0.1.0] – 2026-02-07

### 🎉 Erstveröffentlichung

#### Hinzugefügt

**Backend (ASP.NET Core API)**

- Authentifizierung mit JWT (Login, Registrierung)
- CRUD-Endpunkte für Immobilien (`Properties`)
- CRUD-Endpunkte für Einheiten (`Units`)
- CRUD-Endpunkte für Mieter (`Tenants`)
- CRUD-Endpunkte für Mietverträge (`Leases`)
- CRUD-Endpunkte für Zahlungen (`Payments`)
- CRUD-Endpunkte für Ausgaben (`Expenses`)
- CRUD-Endpunkte für Zählerstände (`MeterReadings`)
- Dashboard-Endpunkt mit Übersichtsdaten
- Swagger/OpenAPI-Dokumentation
- Multi-Tenancy (Eigentümer sehen nur eigene Daten)
- Soft Delete für alle Entities
- Automatische Unit-Status-Verwaltung bei Mietvertragserstellung/-beendigung
- Automatische Verbrauchsberechnung bei Zählerständen

**Architektur**

- Clean Architecture (Core → Infrastructure → API)
- Repository Pattern mit generischem `IRepository<T>`
- Unit of Work Pattern
- Entity Framework Core 9.0 mit SQLite
- PBKDF2-SHA256 Passwort-Hashing mit Salt

**Web-Frontend (Next.js)**

- Next.js 16 mit App Router
- React 19, TypeScript, Tailwind CSS 4
- Login-Seite
- Dashboard-Seite

**Mobile/Desktop (MAUI)**

- .NET MAUI Anwendung
- Login, Dashboard, Properties, Tenants, Payments Seiten
- MVVM-Architecture

**Dokumentation**

- README.md mit Quick-Start
- Architektur-Dokumentation
- Datenmodell mit ER-Diagramm
- API-Referenz aller Endpunkte
- Setup-Anleitung
- Sicherheitsdokumentation
- Glossar der Fachbegriffe
- Contributing-Richtlinien
- Benutzerhandbuch
- Changelog

---

<!--
## [X.Y.Z] – YYYY-MM-DD

### Hinzugefügt
- Neue Features

### Geändert
- Änderungen an bestehendem Verhalten

### Behoben
- Bugfixes

### Entfernt
- Entfernte Features

### Sicherheit
- Sicherheitsrelevante Änderungen

### Veraltet
- Features, die in zukünftigen Versionen entfernt werden
-->
