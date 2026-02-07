# 🏠 ImmobilienVerwalter

**Professionelle Immobilienverwaltungssoftware für private Vermieter und kleine Hausverwaltungen.**

Verwalten Sie Ihre Immobilien, Einheiten, Mieter, Mietverträge, Zahlungen, Ausgaben und Zählerstände – alles in einer Anwendung.

---

## 🏗️ Projektstruktur

```
ImmobilienVerwalter/
├── docs/                        # 📚 Projekt-Dokumentation
├── src/
│   ├── ImmobilienVerwalter.Core/           # Domain-Entities & Interfaces
│   ├── ImmobilienVerwalter.Infrastructure/  # EF Core, Repositories, DB
│   ├── ImmobilienVerwalter.API/            # ASP.NET Core Web API
│   ├── ImmobilienVerwalter.Maui/           # .NET MAUI Mobile/Desktop App
│   └── immobilienverwalter-web/            # Next.js Web-Frontend
└── ImmobilienVerwalter.sln                 # Solution-Datei
```

## 🛠️ Tech-Stack

| Komponente            | Technologie                                      |
| --------------------- | ------------------------------------------------ |
| **Backend API**       | ASP.NET Core 9.0, C#                             |
| **Datenbank**         | SQLite (Datei-basiert, kein externer Server)     |
| **ORM**               | Entity Framework Core 9.0                        |
| **Authentifizierung** | JWT Bearer Token                                 |
| **Web-Frontend**      | Next.js 16, React 19, TypeScript, Tailwind CSS 4 |
| **Mobile/Desktop**    | .NET MAUI                                        |
| **Charts**            | Recharts (Web)                                   |
| **Icons**             | Lucide React                                     |

## ⚡ Quick-Start

### Voraussetzungen

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 20+](https://nodejs.org/)

### 1. Backend starten

```bash
cd src/ImmobilienVerwalter.API
dotnet run
```

Die API läuft unter `http://localhost:5013` (Swagger UI: `http://localhost:5013/swagger`).
Die Datenbank (`ImmobilienVerwalter.db`) wird beim ersten Start automatisch erstellt und migriert.

### 2. Web-Frontend starten

```bash
cd src/immobilienverwalter-web
npm install
npm run dev
```

Das Frontend läuft unter `http://localhost:3000`.

### 3. MAUI App starten

```bash
cd src/ImmobilienVerwalter.Maui
dotnet run -f net9.0-windows10.0.19041.0
```

## 📚 Dokumentation

| Dokument                                     | Beschreibung                                 |
| -------------------------------------------- | -------------------------------------------- |
| [Architektur](docs/ARCHITECTURE.md)          | Clean Architecture, Schichten, Patterns      |
| [Datenmodell](docs/DATA-MODEL.md)            | ER-Diagramm, Entities, Enums, Business Rules |
| [API-Referenz](docs/API.md)                  | Alle REST-Endpunkte, Auth-Flow, DTOs         |
| [Setup-Anleitung](docs/SETUP.md)             | Entwicklungsumgebung einrichten              |
| [Sicherheit](docs/SECURITY.md)               | Authentifizierung, Rollen, CORS              |
| [Glossar](docs/GLOSSARY.md)                  | Fachbegriffe der Immobilienverwaltung        |
| [Contributing](docs/CONTRIBUTING.md)         | Entwicklungsrichtlinien & Doku-Pflege        |
| [Benutzerhandbuch](docs/BENUTZERHANDBUCH.md) | Anleitung für Endbenutzer                    |
| [Changelog](CHANGELOG.md)                    | Versionshistorie                             |

## 📝 Lizenz

Privates Projekt – alle Rechte vorbehalten.
