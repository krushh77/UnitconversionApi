# Unit Conversion API

An ASP.NET Core Web API (.NET 10) that converts numeric values between units of measurement.
Supported categories: **length, mass, temperature, volume, speed**.

## Quick start

Prerequisite: [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
dotnet restore
dotnet test
dotnet run --project src/UnitConversion.Api
```

The API listens on `http://localhost:5080`. In Development the OpenAPI document is at
`http://localhost:5080/openapi/v1.json` (import it into Postman/Insomnia/Swagger UI).

## Endpoints

| Method | Route | Purpose |
|--------|-------|---------|
| POST | `/api/v1/conversions/convert` | Convert a value |
| GET  | `/api/v1/conversions/convert?category=&from=&to=&value=` | Same, via query string |
| GET  | `/api/v1/conversions/categories` | List categories and units |
| GET  | `/api/v1/conversions/categories/{category}` | List units of one category |
| GET  | `/health` | Health check |

### Example

```bash
curl -X POST http://localhost:5080/api/v1/conversions/convert \
  -H "Content-Type: application/json" \
  -d '{ "category": "temperature", "from": "celsius", "to": "fahrenheit", "value": 100 }'
```

```json
{ "category": "temperature", "from": "celsius", "to": "fahrenheit", "value": 100, "result": 212 }
```

Units can be referenced by code, symbol or alias, case-insensitively (`kilometer`, `km`, `KM`).

### Errors

Errors follow RFC 7807 (`application/problem+json`). Example (`400`):

```json
{ "title": "Invalid conversion request", "status": 400,
  "detail": "Unit 'parsec' is not supported in category 'length'.", "code": "unknown_unit" }
```

Codes: `unknown_category`, `unknown_unit`, `invalid_value`. Missing/invalid fields return the standard
validation problem details. Values below a physical minimum (e.g. below absolute zero, negative mass) are rejected.

## Project structure

```
src/UnitConversion.Api/
  Controllers/      HTTP layer only (thin)
  Contracts/        Request/response DTOs
  Domain/           UnitDefinition, UnitCategory, domain exceptions
  Services/         IUnitCatalog (data) and IConversionService (logic)
  Infrastructure/   Cross-cutting: exception -> ProblemDetails mapping
tests/UnitConversion.Tests/   Unit tests + integration tests (WebApplicationFactory)
```

## Design decisions and trade-offs

- **Hub-and-spoke conversion.** Each unit is defined relative to its category's base unit
  (`base = value * factor + offset`). Adding a unit is one line; no N x N conversion table.
  Temperature fits the same model through the offset, so there is no special-case code.
- **Catalog behind an interface.** `IUnitCatalog` is hardcoded in memory today (as allowed), but the
  requirement of "hundreds of units" later means it can be swapped for a database/config-backed
  implementation without touching the service or controllers.
- **Controllers + DTOs + service layer.** Familiar structure for a multi-developer team; business logic is
  testable without HTTP.
- **`double` arithmetic.** Fast and sufficient for general conversions; results are rounded to 12
  significant digits to hide floating-point noise. If exact decimal results (e.g. finance) were needed, switch to `decimal`.
- **Domain exceptions mapped centrally** to ProblemDetails instead of try/catch in controllers.
- **API versioned in the URL** (`/api/v1`) so contracts can evolve.
- **Out of scope for this version:** auth, rate limiting, caching, persistence, Docker/CI.
  The natural next steps would be a DB-backed catalog with caching, and a CI workflow running `dotnet test`.
