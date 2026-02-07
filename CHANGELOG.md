# 📝 Changelog

Alle nennenswerten Änderungen am Projekt werden in dieser Datei dokumentiert.

Das Format basiert auf [Keep a Changelog](https://keepachangelog.com/de/1.0.0/)
und dieses Projekt folgt [Semantic Versioning](https://semver.org/lang/de/).

> **Hinweis:** Bei jedem Release oder bedeutsamen Feature einen neuen Eintrag hinzufügen.

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
- Entity Framework Core 9.0 mit SQL Server
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
