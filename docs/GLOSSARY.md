# 📖 Glossar – Fachbegriffe der Immobilienverwaltung

> **Letzte Aktualisierung:** 2026-02-07  
> **Hinweis:** Bei neuen Domain-Begriffen in der Anwendung dieses Glossar erweitern.

## Immobilie & Einheiten

| Begriff                    | Englisch (Code)        | Erklärung                                                                           |
| -------------------------- | ---------------------- | ----------------------------------------------------------------------------------- |
| **Immobilie**              | `Property`             | Ein Gebäude oder Grundstück, das verwaltet wird                                     |
| **Einheit / Wohneinheit**  | `Unit`                 | Eine vermietbare Einheit innerhalb einer Immobilie (Wohnung, Gewerbe, Garage, etc.) |
| **Mehrfamilienhaus (MFH)** | `Mehrfamilienhaus`     | Gebäude mit mehreren Wohneinheiten                                                  |
| **Einfamilienhaus (EFH)**  | `Einfamilienhaus`      | Gebäude mit einer Wohneinheit                                                       |
| **Mischimmobilie**         | `MischGewerbeWohn`     | Gebäude mit Wohn- und Gewerbeeinheiten                                              |
| **Leerstand**              | `Leer` / `VacantUnits` | Einheit, die aktuell nicht vermietet ist                                            |
| **Belegungsquote**         | `OccupancyRate`        | Prozentsatz der vermieteten Einheiten                                               |
| **Wohnfläche**             | `Area`                 | Fläche einer Einheit in Quadratmetern                                               |
| **Stockwerk**              | `Floor`                | Etage, in der sich die Einheit befindet                                             |

## Mietvertrag & Miete

| Begriff                   | Englisch (Code)        | Erklärung                                                            |
| ------------------------- | ---------------------- | -------------------------------------------------------------------- |
| **Mietvertrag**           | `Lease`                | Vertrag zwischen Vermieter und Mieter über die Nutzung einer Einheit |
| **Mieter**                | `Tenant`               | Person, die eine Einheit mietet                                      |
| **Vermieter**             | `User` / Owner         | Eigentümer der Immobilie                                             |
| **Kaltmiete**             | `ColdRent`             | Grundmiete ohne Nebenkosten                                          |
| **Nebenkosten / NK**      | `AdditionalCosts`      | Vorauszahlung für Betriebskosten (Wasser, Heizung, etc.)             |
| **Warmmiete**             | `TotalRent`            | Kaltmiete + Nebenkosten = Gesamtmiete                                |
| **Kündigungsfrist**       | `NoticePeriodMonths`   | Frist zur Kündigung in Monaten (Standard: 3)                         |
| **Unbefristeter Vertrag** | `EndDate = null`       | Mietvertrag ohne festgelegtes Ende                                   |
| **Befristeter Vertrag**   | `EndDate != null`      | Mietvertrag mit festgelegtem Enddatum                                |
| **Mieterhöhung**          | `LastRentIncreaseDate` | Letzte Anpassung der Kaltmiete                                       |
| **Zahlungstag**           | `PaymentDayOfMonth`    | Tag im Monat, an dem die Miete fällig ist                            |

## Kaution

| Begriff                   | Englisch (Code) | Erklärung                                                    |
| ------------------------- | --------------- | ------------------------------------------------------------ |
| **Kaution / Mietkaution** | `DepositAmount` | Sicherheitsleistung des Mieters (max. 3 Monatskaltmieten)    |
| **Kaution gezahlt**       | `DepositPaid`   | Bereits gezahlter Kautionsbetrag                             |
| **Kautionstatus**         | `DepositStatus` | Status: Ausstehend / Teilweise / Vollständig / Zurückgezahlt |

## Zahlungen

| Begriff              | Englisch (Code) | Erklärung                                               |
| -------------------- | --------------- | ------------------------------------------------------- |
| **Mietzahlung**      | `Payment`       | Einzelner Zahlungseingang                               |
| **Fälligkeitsdatum** | `DueDate`       | Datum, bis zu dem die Zahlung eingehen muss             |
| **Überfällig**       | `Ueberfaellig`  | Zahlung ist nach dem Fälligkeitsdatum nicht eingegangen |
| **Teilzahlung**      | `Teilzahlung`   | Nur ein Teil der erwarteten Summe wurde gezahlt         |
| **Verwendungszweck** | `Reference`     | Buchungstext der Zahlung                                |
| **SEPA-Lastschrift** | `Lastschrift`   | Automatischer Bankeinzug                                |
| **IBAN / BIC**       | `Iban` / `Bic`  | Internationale Bankverbindung des Mieters               |

## Ausgaben & Nebenkosten

| Begriff                   | Englisch (Code)         | Erklärung                                                                 |
| ------------------------- | ----------------------- | ------------------------------------------------------------------------- |
| **Ausgabe / Kosten**      | `Expense`               | Kosten, die für eine Immobilie anfallen                                   |
| **Umlagefähig**           | `IsAllocatable`         | Kosten, die auf Mieter umgelegt werden dürfen (Betriebskosten)            |
| **Nicht umlagefähig**     | -                       | Kosten, die der Vermieter selbst tragen muss (z.B. Zinsen, Abschreibung)  |
| **Betriebskosten**        | -                       | Laufende Kosten einer Immobilie (Wasser, Heizung, Müll, etc.)             |
| **Nebenkostenabrechnung** | `Nebenkostenabrechnung` | Jährliche Abrechnung der tatsächlichen Betriebskosten vs. Vorauszahlungen |
| **Nachzahlung**           | `Nachzahlung`           | Betrag, den der Mieter nach der NK-Abrechnung nachzahlen muss             |
| **Wiederkehrende Kosten** | `IsRecurring`           | Kosten, die regelmäßig anfallen (monatlich, jährlich, etc.)               |
| **Steuerlich absetzbar**  | `IsTaxDeductible`       | Kosten, die in der Steuererklärung geltend gemacht werden können          |
| **AfA (Abschreibung)**    | `Abschreibung`          | Absetzung für Abnutzung – steuerliche Abschreibung des Gebäudewerts       |

## Zählerstände

| Begriff                       | Englisch (Code) | Erklärung                                               |
| ----------------------------- | --------------- | ------------------------------------------------------- |
| **Zählerstand**               | `MeterReading`  | Abgelesener Wert eines Verbrauchszählers                |
| **Zählernummer**              | `MeterNumber`   | Eindeutige Kennung des Zählers                          |
| **Verbrauch**                 | `Consumption`   | Differenz zwischen aktuellem und vorherigem Zählerstand |
| **Ablesung**                  | -               | Vorgang des Zählerstand-Ablesens                        |
| **Heizkostenverteiler (HKV)** | `Heizung`       | Gerät zur Erfassung des Heizverbrauchs                  |

## Dokumente

| Begriff                 | Englisch (Code)       | Erklärung                                             |
| ----------------------- | --------------------- | ----------------------------------------------------- |
| **Übergabeprotokoll**   | `Uebergabeprotokoll`  | Protokoll bei Einzug/Auszug mit Zustandsdokumentation |
| **Energieausweis**      | `Energieausweis`      | Dokument zur energetischen Bewertung eines Gebäudes   |
| **Grundbuchauszug**     | `Grundbuchauszug`     | Amtlicher Nachweis über Eigentumsverhältnisse         |
| **Versicherungspolice** | `Versicherungspolice` | Versicherungsvertrag (Gebäude, Haftpflicht, etc.)     |

## Steuerliche Begriffe

| Begriff               | Erklärung                                                   |
| --------------------- | ----------------------------------------------------------- |
| **Steuernummer**      | Identifikation des Vermieters beim Finanzamt (`TaxId`)      |
| **Einkünfte aus V+V** | Einkünfte aus Vermietung und Verpachtung                    |
| **Werbungskosten**    | Absetzbare Kosten bei Vermietung (Zinsen, AfA, Reparaturen) |
