# 🔌 API-Referenz

> **Letzte Aktualisierung:** 2026-02-07  
> **Hinweis:** Bei neuen Endpunkten, geänderten DTOs oder neuen Controllern dieses Dokument aktualisieren.  
> **Swagger UI:** `https://localhost:5001/swagger` (nur in Development)

## Basis-URL

```
https://localhost:5001/api
```

## Authentifizierung

Alle Endpunkte außer `/api/auth/*` erfordern einen **JWT Bearer Token** im Header:

```
Authorization: Bearer <token>
```

Siehe [SECURITY.md](SECURITY.md) für Details zum Auth-Flow.

---

## 🔐 Auth Controller

**Route:** `/api/auth`  
**Authentifizierung:** Keine (öffentlich)

### `POST /api/auth/login`

Benutzer anmelden und JWT Token erhalten.

**Request Body:**

```json
{
  "email": "max@example.de",
  "password": "MeinPasswort123"
}
```

**Response `200 OK`:**

```json
{
  "token": "eyJhbGci...",
  "expiration": "2026-02-14T12:00:00Z",
  "user": {
    "id": "guid",
    "email": "max@example.de",
    "firstName": "Max",
    "lastName": "Mustermann",
    "fullName": "Max Mustermann",
    "company": "Mustermann GmbH",
    "role": "Vermieter"
  }
}
```

**Fehler:**
| Code | Bedeutung |
|---|---|
| `401` | Ungültige E-Mail oder Passwort |

### `POST /api/auth/register`

Neuen Benutzer registrieren.

**Request Body:**

```json
{
  "email": "max@example.de",
  "password": "MeinPasswort123",
  "firstName": "Max",
  "lastName": "Mustermann",
  "phone": "+49 171 1234567",
  "company": "Mustermann GmbH"
}
```

**Response `201 Created`:** Wie Login-Response  
**Fehler:**
| Code | Bedeutung |
|---|---|
| `409` | E-Mail bereits vergeben |

---

## 📊 Dashboard Controller

**Route:** `/api/dashboard`  
**Authentifizierung:** ✅ Erforderlich

### `GET /api/dashboard`

Dashboard-Übersicht für den angemeldeten Benutzer.

**Response `200 OK`:**

```json
{
  "totalProperties": 3,
  "totalUnits": 12,
  "occupiedUnits": 10,
  "vacantUnits": 2,
  "occupancyRate": 83.3,
  "monthlyIncome": 8500.00,
  "monthlyExpenses": 2100.00,
  "monthlyProfit": 6400.00,
  "yearlyIncome": 102000.00,
  "yearlyExpenses": 25200.00,
  "overduePayments": 1,
  "expiringLeases": 2,
  "recentPayments": [ ... ],
  "expiringLeasesList": [ ... ]
}
```

---

## 🏠 Properties Controller

**Route:** `/api/properties`  
**Authentifizierung:** ✅ Erforderlich  
**Multi-Tenancy:** Nur eigene Immobilien sichtbar (gefiltert nach `OwnerId`)

### `GET /api/properties`

Alle Immobilien des Benutzers abrufen.

### `GET /api/properties/{id}`

Einzelne Immobilie mit Units abrufen.

### `POST /api/properties`

Neue Immobilie erstellen.

**Request Body (`PropertyCreateDto`):**

```json
{
  "name": "Musterhaus",
  "street": "Musterstraße",
  "houseNumber": "42",
  "zipCode": "80331",
  "city": "München",
  "country": "Deutschland",
  "yearBuilt": 1985,
  "totalArea": 450.5,
  "numberOfFloors": 3,
  "type": "Mehrfamilienhaus",
  "purchasePrice": 750000.0,
  "purchaseDate": "2020-01-15"
}
```

### `PUT /api/properties/{id}`

Immobilie aktualisieren. Body: `PropertyUpdateDto` (gleiche Felder wie Create).

### `DELETE /api/properties/{id}`

Immobilie löschen (Soft Delete).

**Antworten:**
| Code | Bedeutung |
|---|---|
| `200` | Erfolg (GET, POST) |
| `204` | Erfolg ohne Body (PUT, DELETE) |
| `404` | Nicht gefunden oder nicht berechtigt |

---

## 🏢 Units Controller

**Route:** `/api/units`  
**Authentifizierung:** ✅ Erforderlich

### `GET /api/units`

Alle Einheiten abrufen.

### `GET /api/units/property/{propertyId}`

Einheiten einer bestimmten Immobilie.

### `GET /api/units/{id}`

Einzelne Einheit mit aktivem Mietvertrag.

### `GET /api/units/vacant?propertyId={guid}`

Leerstehende Einheiten abrufen (optional gefiltert nach Immobilie).

### `POST /api/units`

Neue Einheit erstellen.

**Request Body (`UnitCreateDto`):**

```json
{
  "name": "Wohnung 1 OG links",
  "description": "2-Zimmer-Wohnung mit Balkon",
  "floor": 1,
  "area": 65.5,
  "rooms": 2,
  "type": "Wohnung",
  "targetRent": 750.0,
  "propertyId": "guid"
}
```

### `PUT /api/units/{id}`

Einheit aktualisieren. Body: `UnitUpdateDto` (inkl. `status`).

### `DELETE /api/units/{id}`

Einheit löschen (Soft Delete).

---

## 👤 Tenants Controller

**Route:** `/api/tenants`  
**Authentifizierung:** ✅ Erforderlich

### `GET /api/tenants`

Alle Mieter abrufen.

### `GET /api/tenants/{id}`

Einzelnen Mieter mit Mietverträgen abrufen.

### `GET /api/tenants/search?q={suchbegriff}`

Mieter suchen (Name, E-Mail, etc.).

### `POST /api/tenants`

Neuen Mieter erstellen.

**Request Body (`TenantCreateDto`):**

```json
{
  "firstName": "Anna",
  "lastName": "Müller",
  "email": "anna.mueller@example.de",
  "phone": "+49 89 1234567",
  "mobilePhone": "+49 171 1234567",
  "previousAddress": "Alte Straße 1, 80331 München",
  "iban": "DE89370400440532013000",
  "bic": "COBADEFFXXX",
  "bankName": "Commerzbank",
  "dateOfBirth": "1990-05-15",
  "occupation": "Ingenieurin",
  "monthlyIncome": 4500.0,
  "emergencyContactName": "Peter Müller",
  "emergencyContactPhone": "+49 171 9876543",
  "notes": "Haustier: Katze"
}
```

### `PUT /api/tenants/{id}`

Mieter aktualisieren. Body: `TenantUpdateDto`.

### `DELETE /api/tenants/{id}`

Mieter löschen (Soft Delete).

---

## 📋 Leases Controller

**Route:** `/api/leases`  
**Authentifizierung:** ✅ Erforderlich

### `GET /api/leases`

Alle Mietverträge abrufen.

### `GET /api/leases/active`

Nur aktive Mietverträge.

### `GET /api/leases/{id}`

Einzelnen Mietvertrag mit Zahlungshistorie.

### `GET /api/leases/unit/{unitId}`

Mietverträge einer Einheit.

### `GET /api/leases/tenant/{tenantId}`

Mietverträge eines Mieters.

### `GET /api/leases/expiring?days={90}`

Bald auslaufende Mietverträge (Standard: 90 Tage).

### `POST /api/leases`

Neuen Mietvertrag erstellen. **Setzt die Unit automatisch auf `Vermietet`.**

**Request Body (`LeaseCreateDto`):**

```json
{
  "tenantId": "guid",
  "unitId": "guid",
  "startDate": "2026-03-01",
  "endDate": null,
  "coldRent": 750.0,
  "additionalCosts": 200.0,
  "depositAmount": 2250.0,
  "noticePeriodMonths": 3,
  "paymentDayOfMonth": 1,
  "notes": "Unbefristeter Vertrag"
}
```

### `PUT /api/leases/{id}`

Mietvertrag aktualisieren. **Bei Status `Beendet` wird die Unit automatisch auf `Leer` gesetzt.**

Body: `LeaseUpdateDto` (inkl. `status`, `terminationDate`, `moveOutDate`, `depositPaid`, etc.)

### `DELETE /api/leases/{id}`

Mietvertrag löschen. **Setzt die Unit automatisch auf `Leer`.**

---

## 💰 Payments Controller

**Route:** `/api/payments`  
**Authentifizierung:** ✅ Erforderlich

### `GET /api/payments/lease/{leaseId}`

Zahlungen eines Mietvertrags.

### `GET /api/payments/overdue`

Alle überfälligen Zahlungen.

### `GET /api/payments/month/{year}/{month}`

Zahlungen eines bestimmten Monats.

### `GET /api/payments/income/{year}?month={month}`

Gesamteinnahmen eines Jahres (optional gefiltert nach Monat).

### `GET /api/payments/{id}`

Einzelne Zahlung abrufen.

### `POST /api/payments`

Neue Zahlung erfassen.

**Request Body (`PaymentCreateDto`):**

```json
{
  "leaseId": "guid",
  "amount": 950.0,
  "paymentDate": "2026-02-01",
  "dueDate": "2026-02-01",
  "paymentMonth": 2,
  "paymentYear": 2026,
  "type": "Miete",
  "method": "Ueberweisung",
  "status": "Eingegangen",
  "reference": "Miete Feb 2026",
  "notes": null,
  "expectedAmount": 950.0
}
```

### `PUT /api/payments/{id}`

Zahlung aktualisieren. Body: `PaymentUpdateDto`.

### `DELETE /api/payments/{id}`

Zahlung löschen (Soft Delete).

---

## 💸 Expenses Controller

**Route:** `/api/expenses`  
**Authentifizierung:** ✅ Erforderlich

### `GET /api/expenses`

Alle Ausgaben abrufen.

### `GET /api/expenses/property/{propertyId}`

Ausgaben einer Immobilie.

### `GET /api/expenses/total/{year}?month={month}`

Gesamtausgaben eines Jahres.

### `GET /api/expenses/{id}`

Einzelne Ausgabe abrufen.

### `POST /api/expenses`

Neue Ausgabe erstellen.

**Request Body (`ExpenseCreateDto`):**

```json
{
  "title": "Heizungswartung",
  "description": "Jährliche Wartung Gasheizung",
  "amount": 350.0,
  "date": "2026-01-15",
  "dueDate": "2026-02-15",
  "category": "Wartung",
  "isRecurring": true,
  "recurringInterval": "Jaehrlich",
  "isAllocatable": true,
  "isTaxDeductible": true,
  "vendor": "Heizung Müller GmbH",
  "invoiceNumber": "HM-2026-0042",
  "propertyId": "guid",
  "unitId": null,
  "notes": null
}
```

### `PUT /api/expenses/{id}`

Ausgabe aktualisieren. Body: `ExpenseUpdateDto`.

### `DELETE /api/expenses/{id}`

Ausgabe löschen (Soft Delete).

---

## 📊 MeterReadings Controller

**Route:** `/api/meterreadings`  
**Authentifizierung:** ✅ Erforderlich

### `GET /api/meterreadings/unit/{unitId}`

Zählerstände einer Einheit.

### `GET /api/meterreadings/{id}`

Einzelnen Zählerstand abrufen.

### `POST /api/meterreadings`

Neuen Zählerstand erfassen. **Der vorherige Wert wird automatisch ermittelt.**

**Request Body (`MeterReadingCreateDto`):**

```json
{
  "unitId": "guid",
  "meterType": "Wasser",
  "meterNumber": "WZ-001-2024",
  "value": 1234.56,
  "readingDate": "2026-01-31",
  "notes": "Jahresablesung",
  "photoPath": "/photos/meter_2026-01.jpg"
}
```

### `PUT /api/meterreadings/{id}`

Zählerstand aktualisieren. Body: `MeterReadingUpdateDto`.

### `DELETE /api/meterreadings/{id}`

Zählerstand löschen (Soft Delete).

---

## Allgemeine Fehler-Codes

| HTTP Code                   | Bedeutung                               |
| --------------------------- | --------------------------------------- |
| `200 OK`                    | Erfolgreiche Anfrage                    |
| `201 Created`               | Ressource erfolgreich erstellt          |
| `204 No Content`            | Erfolg (PUT/DELETE ohne Body)           |
| `400 Bad Request`           | Ungültige Anfrage (Validierungsfehler)  |
| `401 Unauthorized`          | Kein oder ungültiger Token              |
| `404 Not Found`             | Ressource nicht gefunden                |
| `409 Conflict`              | Konflikt (z.B. E-Mail bereits vergeben) |
| `500 Internal Server Error` | Serverfehler                            |
