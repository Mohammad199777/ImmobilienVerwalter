# 🏗️ Architektur

> **Letzte Aktualisierung:** 2026-02-07  
> **Hinweis:** Dieses Dokument bei architekturellen Änderungen aktualisieren (neue Schichten, Patterns, Technologien).

## Übersicht

Der ImmobilienVerwalter folgt der **Clean Architecture** (auch bekannt als Onion Architecture). Die Abhängigkeiten zeigen immer **nach innen** – äußere Schichten kennen innere, aber nie umgekehrt.

```
┌─────────────────────────────────────────────────┐
│                  Clients                         │
│  ┌──────────────────┐  ┌─────────────────────┐  │
│  │  Next.js Web App │  │   .NET MAUI App     │  │
│  │  (Port 3000)     │  │   (Windows/Android) │  │
│  └────────┬─────────┘  └──────────┬──────────┘  │
│           │          HTTP/REST     │             │
├───────────┴────────────────────────┴─────────────┤
│              ImmobilienVerwalter.API              │
│         (ASP.NET Core – Port 5013)               │
│  Controllers → DTOs → Services → Middleware      │
├──────────────────────────────────────────────────┤
│         ImmobilienVerwalter.Infrastructure        │
│  EF Core DbContext, Repositories, UnitOfWork     │
├──────────────────────────────────────────────────┤
│            ImmobilienVerwalter.Core               │
│  Entities, Interfaces, Enums, Business Rules     │
├──────────────────────────────────────────────────┤
│            SQLite (ImmobilienVerwalter.db)        │
└──────────────────────────────────────────────────┘
```

## Schichten im Detail

### 1. Core (`ImmobilienVerwalter.Core`)

Die **innerste Schicht** – hat keinerlei externe Abhängigkeiten (keine NuGet-Pakete).

| Ordner                        | Inhalt                                                                                                           |
| ----------------------------- | ---------------------------------------------------------------------------------------------------------------- |
| `Entities/`                   | Domain-Entities: `Property`, `Unit`, `Tenant`, `Lease`, `Payment`, `Expense`, `MeterReading`, `Document`, `User` |
| `Entities/BaseEntity.cs`      | Basisklasse mit `Id` (Guid), `CreatedAt`, `UpdatedAt`, `IsDeleted` (Soft-Delete)                                 |
| `Interfaces/IRepository.cs`   | Generisches Repository-Interface mit CRUD + `FindAsync`, `CountAsync`                                            |
| `Interfaces/IRepositories.cs` | Spezialisierte Interfaces pro Entity (z.B. `IPropertyRepository`, `ILeaseRepository`)                            |
| `Interfaces/IUnitOfWork.cs`   | UnitOfWork-Interface – fasst alle Repositories zusammen                                                          |

**Prinzip:** Core definiert nur WAS gebraucht wird (Interfaces), nicht WIE es umgesetzt wird.

### 2. Infrastructure (`ImmobilienVerwalter.Infrastructure`)

Implementiert die Interfaces aus Core.

| Datei/Ordner                   | Inhalt                                                      |
| ------------------------------ | ----------------------------------------------------------- |
| `Data/AppDbContext.cs`         | EF Core DbContext mit allen DbSets und Konfiguration        |
| `Data/Configurations/`         | Entity-Konfigurationen (Fluent API)                         |
| `Repositories/Repository.cs`   | Generische Repository-Implementierung                       |
| `Repositories/Repositories.cs` | Spezialisierte Repository-Implementierungen                 |
| `UnitOfWork.cs`                | UnitOfWork-Implementierung – zentrale Transaktionssteuerung |

**NuGet-Pakete:**

- `Microsoft.EntityFrameworkCore.Sqlite` 9.0.2
- `Microsoft.EntityFrameworkCore.Tools` 9.0.2

### 3. API (`ImmobilienVerwalter.API`)

ASP.NET Core Web API – die Präsentationsschicht.

| Ordner         | Inhalt                                                                                                    |
| -------------- | --------------------------------------------------------------------------------------------------------- |
| `Controllers/` | 9 API-Controller (Auth, Dashboard, Properties, Units, Tenants, Leases, Payments, Expenses, MeterReadings) |
| `DTOs/Dtos.cs` | Alle Data Transfer Objects (Request/Response) mit Validierungsattributen                                  |
| `Services/`    | Business-Logik-Services (`AuthService`, `DashboardService`)                                               |
| `Middleware/`  | `GlobalExceptionHandler` – zentrale Fehlerbehandlung mit strukturiertem Logging                           |
| `Program.cs`   | App-Konfiguration: DI, Auth, CORS, Swagger, Health Checks, Auto-Migration                                 |

**NuGet-Pakete:**

- `Microsoft.AspNetCore.Authentication.JwtBearer` 9.0.2
- `Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore` 9.0.x
- `Swashbuckle.AspNetCore` 6.9.0 (Swagger/OpenAPI)

### 4. Web-Frontend (`immobilienverwalter-web`)

Next.js Single-Page-Application mit App Router.

| Technologie  | Version          |
| ------------ | ---------------- |
| Next.js      | 16.1.6           |
| React        | 19.2.3           |
| TypeScript   | 5.x              |
| Tailwind CSS | 4.x              |
| Axios        | HTTP-Client      |
| Recharts     | Dashboard-Charts |
| Lucide React | Icons            |

### 5. MAUI App (`ImmobilienVerwalter.Maui`)

.NET MAUI App für Windows und Android mit MVVM-Pattern.

| Ordner        | Inhalt                                                        |
| ------------- | ------------------------------------------------------------- |
| `Views/`      | XAML-Seiten (Login, Dashboard, Properties, Tenants, Payments) |
| `ViewModels/` | MVVM-ViewModels                                               |
| `Services/`   | API-Client Services                                           |
| `Models/`     | Client-seitige Models                                         |
| `Converters/` | XAML Value-Converter                                          |

## Design Patterns

| Pattern                  | Verwendung                                                                                |
| ------------------------ | ----------------------------------------------------------------------------------------- |
| **Repository Pattern**   | Abstraktion des Datenzugriffs (`IRepository<T>` → `Repository<T>`)                        |
| **Unit of Work**         | Transaktionssteuerung über `IUnitOfWork` – ein `SaveChangesAsync()` für alle Änderungen   |
| **DTO Pattern**          | Trennung von Domain-Entities und API-Contracts (`PropertyDto`, `PropertyCreateDto`, etc.) |
| **Soft Delete**          | `IsDeleted`-Flag in `BaseEntity` statt echter Löschung                                    |
| **Dependency Injection** | ASP.NET Core Built-in DI Container                                                        |
| **Global Error Handler** | `GlobalExceptionHandler` Middleware für zentrale Exception-Behandlung                     |
| **MVVM**                 | Model-View-ViewModel in MAUI-App                                                          |

## Dependency Injection (Registrierung)

```csharp
// Datenbankkontext
services.AddDbContext<AppDbContext>(options => options.UseSqlite(...));

// Repositories & UnitOfWork
services.AddScoped<IUnitOfWork, UnitOfWork>();

// Services
services.AddScoped<IAuthService, AuthService>();
services.AddScoped<IDashboardService, DashboardService>();

// Health Checks
services.AddHealthChecks().AddDbContextCheck<AppDbContext>("database");
```

Alle Repositories werden **intern** über den `UnitOfWork` bereitgestellt – nicht einzeln im DI registriert.

## Kommunikation

```
Next.js App ──── HTTP/REST (JSON) ────► ASP.NET Core API ──── EF Core ────► SQLite
MAUI App   ──── HTTP/REST (JSON) ────►        │
                                              │
                                    JWT Bearer Token Auth
```

- **CORS:** Konfigurierbar über `appsettings.json` → `Cors:AllowedOrigins` (Standard: `http://localhost:3000`)
- **Auth:** JWT Token im `Authorization: Bearer <token>` Header (24h gültig)
- **Auto-Migration:** Beim Start wird `db.Database.Migrate()` ausgeführt (mit `EnsureCreated()`-Fallback)
- **Health Check:** `/health` Endpunkt prüft DB-Verbindung
- **Fehlerbehandlung:** `GlobalExceptionHandler` Middleware fängt alle Exceptions und gibt strukturierte Fehler-Responses zurück
