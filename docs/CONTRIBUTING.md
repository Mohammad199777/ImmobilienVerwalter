# 🤝 Contributing – Entwicklungsrichtlinien

> **Letzte Aktualisierung:** 2026-02-07  
> **Hinweis:** Dieses Dokument bei Änderungen an Prozessen, Konventionen oder Doku-Regeln aktualisieren.

## 📚 Dokumentation pflegen – Die goldene Regel

> **Es wird KEINE neue Dokumentation pro Änderung erstellt.**  
> **Stattdessen wird die passende bestehende Doku aktualisiert.**

### Wann welche Doku aktualisieren?

| Was wurde geändert?                            | Welche Doku aktualisieren?                 |
| ---------------------------------------------- | ------------------------------------------ |
| Neue Entity / Entity-Feld / Enum               | [DATA-MODEL.md](DATA-MODEL.md)             |
| Neuer API-Endpunkt / Controller                | [API.md](API.md)                           |
| Neues DTO                                      | [API.md](API.md)                           |
| Neue Schicht / Pattern / Technologie           | [ARCHITECTURE.md](ARCHITECTURE.md)         |
| Auth / Sicherheitsänderung                     | [SECURITY.md](SECURITY.md)                 |
| Neuer Fachbegriff                              | [GLOSSARY.md](GLOSSARY.md)                 |
| Neue Abhängigkeit / Tool                       | [SETUP.md](SETUP.md)                       |
| Neue Benutzer-Funktion                         | [BENUTZERHANDBUCH.md](BENUTZERHANDBUCH.md) |
| Neues Release / Breaking Change                | [CHANGELOG.md](../CHANGELOG.md)            |
| Projekt\u00fcbersicht / Tech-Stack ändert sich | [README.md](../README.md)                  |

### Doku-Pflege Checkliste (bei jedem PR/Commit)

- [ ] Betrifft die Änderung eine bestehende Doku? → Aktualisieren
- [ ] `Letzte Aktualisierung`-Datum in der Doku setzen
- [ ] Keine veralteten Informationen hinterlassen

---

## Git-Workflow

### Branching-Strategie

```
main          ← Stabiler, deploybarer Code
  └── develop ← Integrations-Branch
       ├── feature/properties-filter    ← Neue Features
       ├── feature/tenant-search
       ├── bugfix/payment-calculation   ← Bugfixes
       └── hotfix/auth-token-expired    ← Dringende Fixes
```

### Branch-Namenskonvention

```
feature/<kurze-beschreibung>   → Neue Funktionalität
bugfix/<kurze-beschreibung>    → Fehlerbehebung
hotfix/<kurze-beschreibung>    → Dringender Fix für Production
refactor/<kurze-beschreibung>  → Code-Verbesserung ohne neue Funktion
docs/<kurze-beschreibung>      → Nur Dokumentationsänderungen
```

### Commit-Messages

Format: `<typ>: <kurze Beschreibung>`

```
feat: Nebenkostenabrechnung erstellen
fix: Zahlungsstatus wird nicht korrekt angezeigt
refactor: Repository-Pattern vereinfacht
docs: API-Dokumentation für Payments aktualisiert
style: Code-Formatierung in Controllers
test: Unit-Tests für LeaseService
```

| Typ        | Verwendung                      |
| ---------- | ------------------------------- |
| `feat`     | Neue Funktion                   |
| `fix`      | Bugfix                          |
| `refactor` | Code-Refactoring                |
| `docs`     | Dokumentation                   |
| `style`    | Formatierung (kein Code-Change) |
| `test`     | Tests hinzufügen/ändern         |
| `chore`    | Build, Dependencies, Config     |

---

## Code-Konventionen

### C# (.NET)

| Konvention               | Beispiel                                                |
| ------------------------ | ------------------------------------------------------- |
| **Klassen & Interfaces** | `PascalCase` → `PropertyRepository`, `IUnitOfWork`      |
| **Methoden**             | `PascalCase` → `GetByOwnerIdAsync()`                    |
| **Variablen & Felder**   | `camelCase` → `var propertyList`                        |
| **Private Felder**       | `_camelCase` → `private readonly IUnitOfWork _uow;`     |
| **Async-Methoden**       | Immer mit `Async`-Suffix → `GetAllAsync()`              |
| **DTOs**                 | `{Entity}Dto`, `{Entity}CreateDto`, `{Entity}UpdateDto` |
| **Records für DTOs**     | `public record PropertyDto(...)`                        |
| **Interfaces**           | `I`-Prefix → `IRepository<T>`, `IAuthService`           |
| **Enums**                | `PascalCase`, deutsch → `PropertyType.Mehrfamilienhaus` |
| **Nullable**             | `?` verwenden → `string? Notes`, `DateTime? EndDate`    |

### TypeScript / Next.js

| Konvention                 | Beispiel                                                        |
| -------------------------- | --------------------------------------------------------------- |
| **Komponenten**            | `PascalCase` → `DashboardPage.tsx`                              |
| **Funktionen & Variablen** | `camelCase` → `fetchProperties()`                               |
| **Interfaces / Types**     | `PascalCase` → `interface Property { ... }`                     |
| **Dateien**                | `kebab-case` oder Next.js Konvention → `page.tsx`, `layout.tsx` |
| **CSS**                    | Tailwind CSS Utility Classes                                    |

### Allgemein

- **Sprache im Code:** Englisch (Klassen, Methoden, Variablen)
- **Sprache der Enums:** Deutsch (Domain-Begriffe: `Vermieter`, `Kaltmiete`, etc.)
- **Kommentare:** Deutsch oder Englisch (konsistent innerhalb einer Datei)
- **XML-Docs:** `/// <summary>` für öffentliche Klassen und Interfaces

---

## Architektur-Regeln

1. **Core hat KEINE externen Abhängigkeiten** (keine NuGet-Pakete)
2. **Core definiert Interfaces, Infrastructure implementiert sie**
3. **Controller enthalten keine Business-Logik** – nur Mapping und Delegation
4. **DTOs sind Records** (immutable) – keine Entity-Objekte direkt an die API zurückgeben
5. **Repositories nutzen Soft Delete** (`IsDeleted = true`, nicht physisch löschen)
6. **Alle DB-Änderungen über `IUnitOfWork.SaveChangesAsync()`** – nie direkt über den DbContext
7. **Neue Repositories zum `IUnitOfWork`-Interface hinzufügen** und in `UnitOfWork.cs` registrieren

---

## Neuen Feature hinzufügen – Schritt für Schritt

### Beispiel: Neue Entity "Invoice" hinzufügen

1. **Core** – Entity erstellen:
   - `Entities/Invoice.cs` mit Properties und Enums
   - Interface `IInvoiceRepository` in `Interfaces/IRepositories.cs`
   - `IUnitOfWork` um `IInvoiceRepository Invoices { get; }` erweitern

2. **Infrastructure** – Implementierung:
   - DbSet in `AppDbContext.cs` hinzufügen
   - Entity-Konfiguration in `Data/Configurations/`
   - `InvoiceRepository` in `Repositories/Repositories.cs`
   - Repository in `UnitOfWork.cs` registrieren

3. **API** – Endpunkte:
   - DTOs in `DTOs/Dtos.cs`: `InvoiceDto`, `InvoiceCreateDto`, `InvoiceUpdateDto`
   - `Controllers/InvoicesController.cs` erstellen
   - ggf. Service in `Services/`

4. **Dokumentation aktualisieren:**
   - [DATA-MODEL.md](DATA-MODEL.md) → Entity, Felder, Beziehungen, Enums
   - [API.md](API.md) → Neue Endpunkte
   - [GLOSSARY.md](GLOSSARY.md) → Neue Fachbegriffe
   - [CHANGELOG.md](../CHANGELOG.md) → Neues Feature dokumentieren

---

## PR-Review Checkliste

- [ ] Code kompiliert fehlerfrei (`dotnet build`)
- [ ] Keine Warnungen
- [ ] Naming-Konventionen eingehalten
- [ ] Architektur-Regeln beachtet (Core → Infrastructure → API)
- [ ] DTOs verwendet (keine Entities als API-Response)
- [ ] Dokumentation aktualisiert (falls betroffen)
- [ ] Keine Secrets im Code
- [ ] Soft Delete statt physischer Löschung
