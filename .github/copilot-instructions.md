# Copilot Instructions — ImmobilienVerwalter

## Architecture

Clean Architecture with three .NET layers plus two frontend clients, all communicating over REST/JSON:

- **Core** (`ImmobilienVerwalter.Core`) — Entities, interfaces, enums. **Zero NuGet dependencies.**
- **Infrastructure** (`ImmobilienVerwalter.Infrastructure`) — EF Core (SQLite), repositories, UnitOfWork.
- **API** (`ImmobilienVerwalter.API`) — ASP.NET Core 9 controllers, DTOs, services, middleware. Port `5013`.
- **Web** (`immobilienverwalter-web`) — Next.js 16 / React 19 / TypeScript / Tailwind CSS 4. Port `3000`.
- **MAUI** (`ImmobilienVerwalter.Maui`) — .NET MAUI desktop/mobile client (MVVM).

Database is **SQLite** (file `ImmobilienVerwalter.db`, auto-created on first run). No external DB server.

## Key Conventions

- **Language rule:** Code identifiers in **English**, domain enum values and user-facing strings (validation messages, error responses) in **German**.
- **Soft delete only** — never physically remove records. Set `IsDeleted = true` via `BaseEntity`. All queries are globally filtered.
- **No AutoMapper** — each controller has a private `MapToDto()` method for manual entity→DTO mapping.
- **All DTOs are C# records** defined in the single file `DTOs/Dtos.cs`. Naming: `XxxDto`, `XxxCreateDto`, `XxxUpdateDto`.
- **All concrete repositories** live in `Infrastructure/Repositories/Repositories.cs` (single file).
- **Entity configurations** live in `Infrastructure/Data/Configurations/EntityConfigurations.cs` (single file).
- **UnitOfWork** is the only way to persist — repos never call `SaveChanges`; always call `_unitOfWork.SaveChangesAsync()` after mutations.
- Repositories are accessed through `IUnitOfWork` properties, not registered individually in DI.

## Controller Pattern

Every controller: `[ApiController]`, `[Route("api/[controller]")]`, `[Authorize]`. Inject `IUnitOfWork` + `ILogger<T>`.

```csharp
private Guid GetCurrentUserId()  // extracts user from JWT claims
// Every read/write must verify entity.UserId == currentUserId → return 404 if not owned
```

- GET → `Ok(dto)` / `Ok(list)`
- POST → `CreatedAtAction(...)` (201)
- PUT/DELETE → `NoContent()` (204)
- Business rule violations → `BadRequest("German message")`

## Adding a New Entity (Checklist)

1. **Core:** Create entity in `Entities/`, add `IXxxRepository` to `Interfaces/IRepositories.cs`, add property to `IUnitOfWork`.
2. **Infrastructure:** Add `DbSet` to `AppDbContext`, add Fluent API config to `EntityConfigurations.cs`, implement repository in `Repositories/Repositories.cs`, wire in `UnitOfWork.cs`.
3. **API:** Add DTOs (records) to `DTOs/Dtos.cs`, create controller in `Controllers/`, add service only if complex logic needed.
4. **Docs:** Update `DATA-MODEL.md`, `API.md`, `GLOSSARY.md`, and `CHANGELOG.md`.

## Frontend (Next.js)

- App Router: `/login` (public), `/dashboard/*` (protected via layout auth guard).
- API client in `src/lib/api.ts` — Axios with JWT from `localStorage`, 401 interceptor redirects to `/login`.
- Enums are sent as **numbers** matching C# enum integer values.
- Dates sent as ISO strings.
- Default API base URL: `NEXT_PUBLIC_API_URL` env var (fallback `https://localhost:5001/api`).

## Developer Workflow

```bash
# Backend
cd src/ImmobilienVerwalter.API
dotnet run                        # API at http://localhost:5013, Swagger at /swagger

# Frontend
cd src/immobilienverwalter-web
npm install && npm run dev        # http://localhost:3000

# EF Core Migrations (from API project dir)
dotnet ef migrations add <Name> --project ../ImmobilienVerwalter.Infrastructure/ImmobilienVerwalter.Infrastructure.csproj

# MAUI (optional)
cd src/ImmobilienVerwalter.Maui
dotnet run -f net9.0-windows10.0.19041.0
```

## Git Conventions

- Branches: `feature/`, `bugfix/`, `hotfix/`, `refactor/`, `docs/`
- Commits: `feat:`, `fix:`, `refactor:`, `docs:`, `style:`, `test:`, `chore:`
