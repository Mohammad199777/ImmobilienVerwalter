# 📘 Benutzerhandbuch – ImmobilienVerwalter

> **Letzte Aktualisierung:** 2026-02-07  
> **Hinweis:** Bei neuen Funktionen oder geänderten Benutzeroberflächen dieses Handbuch aktualisieren.

## Inhaltsverzeichnis

1. [Einführung](#1-einführung)
2. [Registrierung & Anmeldung](#2-registrierung--anmeldung)
3. [Dashboard](#3-dashboard)
4. [Immobilien verwalten](#4-immobilien-verwalten)
5. [Einheiten verwalten](#5-einheiten-verwalten)
6. [Mieter verwalten](#6-mieter-verwalten)
7. [Mietverträge verwalten](#7-mietverträge-verwalten)
8. [Zahlungen erfassen](#8-zahlungen-erfassen)
9. [Ausgaben verwalten](#9-ausgaben-verwalten)
10. [Zählerstände erfassen](#10-zählerstände-erfassen)

---

## 1. Einführung

Der **ImmobilienVerwalter** ist eine Software für private Vermieter und kleine Hausverwaltungen. Sie hilft Ihnen, Ihre Immobilien, Mieter, Mietverträge, Zahlungseingänge, Ausgaben und Zählerstände zentral zu verwalten.

### Verfügbare Plattformen

| Plattform    | Beschreibung                                  |
| ------------ | --------------------------------------------- |
| **Web-App**  | Browser-basiert unter `http://localhost:3000` |
| **MAUI-App** | Desktop-/Mobile-Anwendung (Windows, Android)  |

### Begriffe in der Anwendung

Für eine vollständige Übersicht der Fachbegriffe siehe [Glossar](GLOSSARY.md).

---

## 2. Registrierung & Anmeldung

### Konto erstellen

1. Öffnen Sie die Anwendung
2. Klicken Sie auf **Registrieren**
3. Geben Sie Ihre Daten ein:
   - **E-Mail** (wird als Login verwendet)
   - **Passwort** (sicher wählen)
   - **Vorname** und **Nachname**
   - **Telefonnummer** (optional)
   - **Firma / Hausverwaltung** (optional)
4. Klicken Sie auf **Konto erstellen**

### Anmelden

1. Geben Sie Ihre **E-Mail** und Ihr **Passwort** ein
2. Klicken Sie auf **Anmelden**
3. Sie werden zum Dashboard weitergeleitet

> **Hinweis:** Ihre Sitzung ist 7 Tage gültig. Danach müssen Sie sich erneut anmelden.

---

## 3. Dashboard

Das Dashboard gibt Ihnen eine Übersicht über Ihren gesamten Immobilienbestand:

| Kennzahl                  | Beschreibung                                     |
| ------------------------- | ------------------------------------------------ |
| **Immobilien**            | Anzahl Ihrer verwalteten Immobilien              |
| **Einheiten**             | Gesamtzahl aller Einheiten                       |
| **Vermietet / Leer**      | Belegungs- und Leerstandsübersicht               |
| **Belegungsquote**        | Prozentsatz der vermieteten Einheiten            |
| **Monatliche Einnahmen**  | Summe aller Mietzahlungen im aktuellen Monat     |
| **Monatliche Ausgaben**   | Summe aller Kosten im aktuellen Monat            |
| **Monatlicher Gewinn**    | Einnahmen minus Ausgaben                         |
| **Überfällige Zahlungen** | Anzahl nicht eingegangener Mieten                |
| **Auslaufende Verträge**  | Mietverträge, die in den nächsten 90 Tagen enden |

---

## 4. Immobilien verwalten

### Neue Immobilie anlegen

1. Navigieren Sie zu **Immobilien**
2. Klicken Sie auf **Neue Immobilie**
3. Füllen Sie die Felder aus:

| Feld         | Pflicht | Beschreibung                            |
| ------------ | ------- | --------------------------------------- |
| Name         | ✅      | Bezeichnung (z.B. "Musterstraße 42")    |
| Straße       | ✅      | Straßenname                             |
| Hausnummer   | ✅      | Hausnummer                              |
| PLZ          | ✅      | Postleitzahl                            |
| Stadt        | ✅      | Ort                                     |
| Land         | ❌      | Standard: Deutschland                   |
| Baujahr      | ❌      | Baujahr des Gebäudes                    |
| Gesamtfläche | ❌      | In Quadratmetern                        |
| Stockwerke   | ❌      | Anzahl Etagen                           |
| Typ          | ✅      | Einfamilienhaus, Mehrfamilienhaus, etc. |
| Kaufpreis    | ❌      | Kaufpreis in Euro                       |
| Kaufdatum    | ❌      | Datum des Erwerbs                       |

4. Klicken Sie auf **Speichern**

### Immobilie bearbeiten

1. Klicken Sie auf eine Immobilie in der Liste
2. Ändern Sie die gewünschten Felder
3. Klicken Sie auf **Speichern**

### Immobilie löschen

1. Öffnen Sie die Immobilie
2. Klicken Sie auf **Löschen**
3. Bestätigen Sie die Löschung

> **Hinweis:** Gelöschte Immobilien werden nicht endgültig entfernt, sondern archiviert (Soft Delete).

---

## 5. Einheiten verwalten

Einheiten sind die einzelnen vermietbaren Einheiten innerhalb einer Immobilie (Wohnungen, Gewerbe, Garagen, etc.).

### Neue Einheit anlegen

1. Öffnen Sie eine **Immobilie**
2. Klicken Sie auf **Neue Einheit**
3. Füllen Sie die Felder aus:

| Feld         | Pflicht | Beschreibung                                 |
| ------------ | ------- | -------------------------------------------- |
| Name         | ✅      | z.B. "Wohnung 1 OG links"                    |
| Beschreibung | ❌      | Freitext                                     |
| Stockwerk    | ❌      | Etage                                        |
| Fläche (m²)  | ✅      | Wohn-/Nutzfläche                             |
| Zimmer       | ❌      | Anzahl Zimmer                                |
| Typ          | ✅      | Wohnung, Gewerbe, Garage, Stellplatz, Keller |
| Soll-Miete   | ✅      | Gewünschte Kaltmiete                         |

4. Klicken Sie auf **Speichern**

### Status einer Einheit

| Status                | Bedeutung                             |
| --------------------- | ------------------------------------- |
| 🟢 **Vermietet**      | Einheit hat einen aktiven Mietvertrag |
| 🔴 **Leer**           | Einheit steht leer (Leerstand)        |
| 🟡 **In Renovierung** | Einheit wird gerade renoviert         |
| 🔵 **Eigennutzung**   | Vom Eigentümer selbst genutzt         |

> Der Status wird automatisch aktualisiert, wenn ein Mietvertrag erstellt oder beendet wird.

---

## 6. Mieter verwalten

### Neuen Mieter anlegen

1. Navigieren Sie zu **Mieter**
2. Klicken Sie auf **Neuer Mieter**
3. Füllen Sie die Daten aus:

| Bereich            | Felder                                                         |
| ------------------ | -------------------------------------------------------------- |
| **Persönlich**     | Vorname, Nachname, E-Mail, Telefon, Mobil, Geburtsdatum, Beruf |
| **Bankverbindung** | IBAN, BIC, Bankname (für SEPA-Lastschrift)                     |
| **Finanziell**     | Monatliches Einkommen                                          |
| **Notfallkontakt** | Name, Telefonnummer                                            |
| **Sonstiges**      | Vorherige Adresse, Notizen                                     |

### Mieter suchen

Nutzen Sie die **Suche** oben auf der Mieterseite. Es wird nach Name, E-Mail und weiteren Feldern gesucht.

---

## 7. Mietverträge verwalten

Ein Mietvertrag verbindet einen **Mieter** mit einer **Einheit**.

### Neuen Mietvertrag erstellen

1. Navigieren Sie zu **Mietverträge**
2. Klicken Sie auf **Neuer Vertrag**
3. Wählen Sie:
   - **Mieter** (muss vorher angelegt sein)
   - **Einheit** (muss leer sein)
4. Geben Sie die Vertragsdaten ein:

| Feld                | Beschreibung                                 |
| ------------------- | -------------------------------------------- |
| **Beginn**          | Startdatum des Mietvertrags                  |
| **Ende**            | Leer lassen für unbefristeten Vertrag        |
| **Kaltmiete**       | Nettomiete in Euro                           |
| **Nebenkosten**     | Monatliche NK-Vorauszahlung                  |
| **Kaution**         | Kautionsbetrag (üblicherweise 3 × Kaltmiete) |
| **Kündigungsfrist** | In Monaten (Standard: 3)                     |
| **Zahlungstag**     | Tag im Monat für Mietzahlung                 |

5. Klicken Sie auf **Speichern**

> Die Einheit wird automatisch auf **Vermietet** gesetzt.

### Mietvertrag kündigen

1. Öffnen Sie den Mietvertrag
2. Setzen Sie den Status auf **Gekündigt**
3. Tragen Sie das **Kündigungsdatum** ein
4. Bei Auszug: Tragen Sie das **Auszugsdatum** ein und setzen den Status auf **Beendet**

> Bei Beendigung wird die Einheit automatisch auf **Leer** zurückgesetzt.

### Vertragsstatus

| Status           | Bedeutung                                |
| ---------------- | ---------------------------------------- |
| 📝 **Entwurf**   | Vertrag noch nicht aktiv                 |
| ✅ **Aktiv**     | Laufender Mietvertrag                    |
| ⚠️ **Gekündigt** | Vertrag wurde gekündigt, läuft aber noch |
| ❌ **Beendet**   | Vertrag ist abgeschlossen                |

---

## 8. Zahlungen erfassen

### Neue Zahlung erfassen

1. Navigieren Sie zu **Zahlungen** oder öffnen Sie einen Mietvertrag
2. Klicken Sie auf **Neue Zahlung**
3. Geben Sie ein:

| Feld                 | Beschreibung                             |
| -------------------- | ---------------------------------------- |
| **Mietvertrag**      | Zu welchem Vertrag gehört die Zahlung    |
| **Betrag**           | Eingegangener Betrag                     |
| **Zahlungsdatum**    | Datum des Geldeingangs                   |
| **Fälligkeitsdatum** | Wann die Zahlung fällig war              |
| **Monat / Jahr**     | Für welchen Monat wird gezahlt           |
| **Typ**              | Miete, Kaution, Nachzahlung, Rückzahlung |
| **Zahlungsart**      | Überweisung, Lastschrift, Bar, PayPal    |
| **Verwendungszweck** | Buchungstext                             |

### Zahlungsstatus

| Status             | Bedeutung                           |
| ------------------ | ----------------------------------- |
| ✅ **Eingegangen** | Zahlung ist vollständig eingegangen |
| ⏳ **Ausstehend**  | Zahlung wird erwartet               |
| 🔴 **Überfällig**  | Fälligkeitsdatum überschritten      |
| ⚠️ **Teilzahlung** | Nur ein Teil der Miete gezahlt      |
| ❌ **Storniert**   | Zahlung wurde storniert             |

### Überfällige Zahlungen prüfen

Über **Zahlungen → Überfällig** sehen Sie alle Mieter, die ihre Miete noch nicht gezahlt haben.

---

## 9. Ausgaben verwalten

### Neue Ausgabe erfassen

1. Navigieren Sie zu **Ausgaben**
2. Klicken Sie auf **Neue Ausgabe**
3. Geben Sie ein:

| Feld                      | Beschreibung                                |
| ------------------------- | ------------------------------------------- |
| **Titel**                 | Kurze Beschreibung (z.B. "Heizungswartung") |
| **Betrag**                | Kosten in Euro                              |
| **Datum**                 | Datum der Ausgabe                           |
| **Kategorie**             | Reparatur, Versicherung, Grundsteuer, etc.  |
| **Immobilie**             | Zuordnung zu einer Immobilie                |
| **Einheit**               | Optional: Zuordnung zu einer Einheit        |
| **Umlagefähig?**          | Kann auf Mieter umgelegt werden?            |
| **Steuerlich absetzbar?** | Als Werbungskosten absetzbar?               |
| **Wiederkehrend?**        | Monatlich, quartalsweise, jährlich          |
| **Dienstleister**         | Name des Handwerkers / Firma                |
| **Rechnungsnummer**       | Für die Buchführung                         |

### Umlagefähige vs. nicht umlagefähige Kosten

- **Umlagefähig** (✅): Wasser, Heizung, Müll, Grundsteuer, Versicherung, Reinigung, etc.
- **Nicht umlagefähig** (❌): Zinsen, Abschreibung, Renovierung, Bankgebühren

> Umlagefähige Kosten werden für die jährliche **Nebenkostenabrechnung** berücksichtigt.

---

## 10. Zählerstände erfassen

### Neuen Zählerstand eintragen

1. Öffnen Sie eine **Einheit**
2. Navigieren Sie zu **Zählerstände**
3. Klicken Sie auf **Neuer Zählerstand**
4. Geben Sie ein:

| Feld             | Beschreibung                            |
| ---------------- | --------------------------------------- |
| **Zählerart**    | Wasser, Warmwasser, Gas, Strom, Heizung |
| **Zählernummer** | Nummer auf dem Zähler                   |
| **Zählerstand**  | Abgelesener Wert                        |
| **Ablesedatum**  | Datum der Ablesung                      |
| **Notizen**      | Optional: Anmerkungen                   |
| **Foto**         | Optional: Foto des Zählers              |

> Der **Verbrauch** wird automatisch berechnet: Aktueller Wert minus vorheriger Wert.

### Wann Zählerstände ablesen?

- **Jährlich:** Für die Nebenkostenabrechnung (Stichtag: meist 31.12.)
- **Bei Einzug/Auszug:** Für die Abrechnung mit dem Mieter
- **Zwischendurch:** Zur Verbrauchskontrolle

---

## Häufig gestellte Fragen (FAQ)

### Wie erstelle ich eine Nebenkostenabrechnung?

Die Nebenkostenabrechnung wird aus den umlagefähigen Ausgaben und den Zählerständen berechnet. _Diese Funktion befindet sich in Entwicklung._

### Kann ich Daten exportieren?

_Der Datenexport (CSV, PDF) befindet sich in Entwicklung._

### Werden meine Daten gesichert?

Die Daten werden in einer SQL Server Datenbank gespeichert. Regelmäßige Backups sollten eingerichtet werden.

### Kann ich mehrere Benutzer haben?

Ja, jeder Benutzer sieht nur seine eigenen Immobilien. Ein Admin kann alle Daten einsehen.

### Was passiert wenn ich etwas lösche?

Gelöschte Einträge werden archiviert (Soft Delete) und können bei Bedarf wiederhergestellt werden.
