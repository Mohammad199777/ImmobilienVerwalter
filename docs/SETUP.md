# 🛠️ Setup-Anleitung

> **Letzte Aktualisierung:** 2026-02-07  
> **Hinweis:** Bei neuen Abhängigkeiten, Konfigurationsänderungen oder Tool-Updates dieses Dokument aktualisieren.

## Voraussetzungen

| Tool                            | Version | Download                                                                 |
| ------------------------------- | ------- | ------------------------------------------------------------------------ |
| .NET SDK                        | 9.0+    | [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/9.0) |
| Node.js                         | 20+     | [nodejs.org](https://nodejs.org/)                                        |
| Visual Studio 2022 oder VS Code | Aktuell | [visualstudio.com](https://visualstudio.com/)                            |
| Git                             | 2.x     | [git-scm.com](https://git-scm.com/)                                      |

> **Hinweis:** Eine externe Datenbank ist nicht erforderlich – die App nutzt **SQLite** (Datei-basiert, wird automatisch erstellt).

### Optional (für MAUI)

| Tool               | Version                        |
| ------------------ | ------------------------------ |
| .NET MAUI Workload | `dotnet workload install maui` |
| Android SDK        | Via Visual Studio Installer    |

---

## 1. Repository klonen

```bash
git clone <repository-url>
cd ImmobilienVerwalter
```

## 2. Datenbank

Die Anwendung nutzt **SQLite** als Datenbank. Die Datenbankdatei `ImmobilienVerwalter.db` wird beim ersten API-Start **automatisch erstellt** und migriert – kein externer Datenbankserver erforderlich.

## 3. Backend (API) starten

```bash
cd src/ImmobilienVerwalter.API
dotnet restore
dotnet run
```

**Die API ist verfügbar unter:**

- API: `http://localhost:5013`
- Swagger UI: `http://localhost:5013/swagger`
- Health Check: `http://localhost:5013/health`

### Konfiguration

Die Konfiguration liegt in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=ImmobilienVerwalter.db"
  },
  "Jwt": {
    "Issuer": "ImmobilienVerwalter",
    "Audience": "ImmobilienVerwalterApp"
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:3000"]
  }
}
```

> ⚠️ **JWT Secret:** In Development wird der Secret aus `appsettings.Development.json` geladen. In Production über die Umgebungsvariable `JWT_SECRET` setzen! Siehe [SECURITY.md](SECURITY.md).

## 4. Web-Frontend starten

```bash
cd src/immobilienverwalter-web
npm install
npm run dev
```

**Das Frontend ist verfügbar unter:**

- Web: `http://localhost:3000`

### Umgebungsvariablen (optional)

Falls die API nicht auf dem Standard-Port läuft, eine `.env.local` erstellen:

```env
NEXT_PUBLIC_API_URL=http://localhost:5013/api
```

## 5. MAUI App starten (optional)

```bash
# MAUI Workload installieren (einmalig)
dotnet workload install maui

# Windows-App starten
cd src/ImmobilienVerwalter.Maui
dotnet run -f net9.0-windows10.0.19041.0
```

## Tipps für die Entwicklung

### Beide Projekte gleichzeitig starten

**Terminal 1 – API:**

```bash
cd src/ImmobilienVerwalter.API
dotnet watch run
```

**Terminal 2 – Web:**

```bash
cd src/immobilienverwalter-web
npm run dev
```

### Datenbank zurücksetzen

Die SQLite-Datenbankdatei kann einfach gelöscht werden – sie wird beim nächsten Start neu erstellt und migriert:

```bash
# API stoppen, dann:
cd src/ImmobilienVerwalter.API
del ImmobilienVerwalter.db
# API neu starten – DB wird automatisch erstellt
```

### VS Code empfohlene Extensions

| Extension                 | Zweck                  |
| ------------------------- | ---------------------- |
| C# Dev Kit                | .NET-Entwicklung       |
| REST Client               | `.http`-Dateien testen |
| ESLint                    | TypeScript Linting     |
| Tailwind CSS IntelliSense | Tailwind Autocomplete  |
| Prettier                  | Code-Formatierung      |

### API testen

Die Datei `src/ImmobilienVerwalter.API/ImmobilienVerwalter.API.http` enthält vorgefertigte HTTP-Requests für die REST Client Extension.

## Projektstruktur

```
ImmobilienVerwalter/
├── docs/                                    # Dokumentation
├── src/
│   ├── ImmobilienVerwalter.Core/            # Domain (Entities, Interfaces)
│   │   ├── Entities/                        # Domain-Objekte
│   │   └── Interfaces/                      # Repository- & UoW-Interfaces
│   ├── ImmobilienVerwalter.Infrastructure/  # Datenzugriff
│   │   ├── Data/                            # DbContext & Konfigurationen
│   │   └── Repositories/                    # Repository-Implementierungen
│   ├── ImmobilienVerwalter.API/             # Web API
│   │   ├── Controllers/                     # API-Endpunkte
│   │   ├── DTOs/                            # Data Transfer Objects
│   │   └── Services/                        # Business-Logik
│   ├── ImmobilienVerwalter.Maui/            # Mobile/Desktop App
│   │   ├── Views/                           # XAML-Seiten
│   │   ├── ViewModels/                      # MVVM-ViewModels
│   │   └── Services/                        # API-Client
│   └── immobilienverwalter-web/             # Web-Frontend
│       └── src/app/                         # Next.js App Router
└── ImmobilienVerwalter.sln                  # Solution-Datei
```

## Häufige Probleme

### API startet nicht – Datenbank-Fehler

```
SqliteException: SQLite Error...
```

**Lösung:** Die Datenbankdatei `ImmobilienVerwalter.db` im API-Verzeichnis löschen und die API neu starten. Die Migration wird automatisch ausgeführt.

### CORS-Fehler im Browser

```
Access to fetch has been blocked by CORS policy
```

**Lösung:** Sicherstellen, dass das Frontend auf `http://localhost:3000` läuft (konfiguriert in `Program.cs`).

### JWT Token abgelaufen

Token sind 24 Stunden gültig. Bei Ablauf erneut über `/api/auth/login` anmelden.
