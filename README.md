# Skincare Inventory App

A skincare inventory management system built with ASP.NET Core, allowing users to track owned products, maintain a wishlist, log ingredient reactions, and receive email reminders before products expire. Product data is populated automatically via a scheduled ETL pipeline that pulls from the Open Beauty Facts public API.

## Tech Stack

- **Backend:** ASP.NET Core Web API (.NET 10)
- **Database:** SQLite (via Entity Framework Core)
- **Authentication:** Custom JWT bearer authentication (no ASP.NET Core Identity)
- **Email:** MailKit (SMTP)
- **External API:** Open Beauty Facts (`world.openbeautyfacts.org`)

## Architecture

The solution is organized into four layered projects:

```
Domain      — entities, enums, DTOs, config, and pure domain services (no external dependencies)
Repository  — EF Core DbContext, generic repository implementation
Service     — business logic, use-case orchestration, external API clients, background jobs
Web         — controllers, request/response DTOs, mappers, composition root (Program.cs)
```

Dependencies flow inward: `Web` depends on `Service` and `Repository`; `Service` and `Repository` both depend on `Domain`. `Domain` has no dependencies on any other project.

Each domain area (`Product`, `InventoryItem`, `WishlistItem`, `Category`, `Ingredient`, `IngredientReaction`) follows the same pattern across layers: a `Service` interface + implementation, a `Web` mapper that translates between the domain model and request/response records, and a thin controller.

## Data Model

| Entity | Description |
|---|---|
| `User` | Application user; plain entity, not tied to ASP.NET Core Identity |
| `Product` | Shared catalog item (name, brand, barcode, image); populated via manual entry or the ETL pipeline |
| `Category` | Fixed taxonomy (Skincare, Bodycare, Cosmetics, Sun Care); seeded at startup, not user-creatable |
| `ProductCategory` | Many-to-many join between `Product` and `Category` (composite key) |
| `Ingredient` | Shared ingredient reference data, parsed from product ingredient lists |
| `InventoryItem` | A specific product a specific user owns, with purchase/opened/expiration dates, PAO tracking, rating, and status |
| `WishlistItem` | A product a specific user wants, with its own status lifecycle |
| `IngredientReaction` | **Ternary relation** — links `User` + `Product` + `Ingredient`, recording a reaction type and severity |
| `EtlSyncLog` | Audit record of each ETL sync run (success/failure, counts, timestamps) |

### Ternary Relation

`IngredientReaction` connects three independent entities (`User`, `Product`, `Ingredient`) in a single relation, each with its own foreign key, plus attributes of its own (`ReactionType`, `ReactionSeverity`, `Note`). This powers a business-logic feature: before a user adds a product, the system can check whether that product contains any ingredient the user has previously reacted to, via `GetConflictingIngredientsAsync`.

## Business Logic Highlights

### PAO-Aware Expiration Calculation

Skincare products are often labeled with a "Period After Opening" (PAO) symbol (e.g. "12M") rather than a fixed expiration date. `IExpirationCalculator` (a pure domain service, dependency-free) computes the effective expiration as the earlier of:
- the product's printed expiration date, or
- `OpenedDate + PaoMonths`

If only one value is available, that one is used; if neither is set, the item is treated as having no known expiration.

### Inventory Item State Machine

`InventoryItem.ProductStatus` follows a fixed set of transitions:

```
Active → Opened → Finished
Active → Discarded
Opened → Discarded
Active/Opened → Expired   (system-driven, via the expiration check job)
```

`Finished`, `Discarded`, and `Expired` are terminal — no transition exists back out of them. Each transition is exposed as its own service method (`OpenProductAsync`, `FinishProductAsync`, `DiscardProductAsync`) with its own guard clause, rather than a single generic status-setter, so illegal transitions are rejected explicitly.

## Integrations

### ETL Pipeline (Extract, Transform, Load)

**Source:** Open Beauty Facts public search API (`/api/v2/search`), queried per category tag (`en:face-care`, `en:body-care`, `en:cosmetics`, `en:sun-care`).

- **Extract:** `ExternalProductApi` calls the search endpoint, paginating through results per configured category.
- **Transform:** `ExternalProductTransformer` maps the raw API response into `Product` entities, and parses `ingredients_text` into a list of individual ingredient names.
- **Load:** `EtlSyncService` upserts products by barcode, links parsed ingredients via the `Ingredient` many-to-many relation, and tags each product with a `Category` based on the search that found it.

Every run is recorded in `EtlSyncLog` (start/end time, success flag, error message if any, counts of products imported/updated/skipped).

The pipeline runs automatically via a hosted `BackgroundService` (`BackgroundEtlSyncJob`) and can also be triggered on demand:

```
POST /api/etl/sync
```

Configuration (`appsettings.json`, `ProductEtl` section) controls which categories are searched, page size, and max pages per run, respecting Open Beauty Facts' documented rate limit (10 requests/minute for search).

### External API Integration

The same `IExternalProductApi` abstraction used by the ETL job is the external API integration point — Open Beauty Facts is a live, third-party, publicly documented REST API (not a static dataset), queried at runtime.

### Email Integration

`IEmailSender` (implemented via MailKit/SMTP) sends expiration reminder emails. A daily background job (`ExpirationCheckJob`) scans all active/opened inventory items:

- If the effective expiration date has passed, the item's status is transitioned to `Expired`.
- If the item is expiring within the next 7 days and no reminder has been sent yet, an email is sent to the owning user and `ReminderSent` is flagged to prevent duplicate notifications.

Manual trigger for testing:

```
POST /api/expirationcheck/run
```

### Authentication

JWT bearer authentication is implemented from scratch against the application's own `User` entity — no ASP.NET Core Identity. Passwords are hashed using `Microsoft.AspNetCore.Identity`'s standalone `PasswordHasher<T>` utility, used purely as a hashing algorithm without pulling in Identity's broader architecture (`IdentityUser`, `IdentityDbContext`, etc.).

```
POST /api/auth/register
POST /api/auth/login
```

A successful login returns a JWT containing the user's ID as a `ClaimTypes.NameIdentifier` claim, which `ICurrentUserService` reads on every authenticated request to scope data to the logged-in user.

## Getting Started

### Prerequisites
- .NET 10 SDK
- EF Core CLI tools: `dotnet tool install --global dotnet-ef`

### Setup

1. Restore dependencies:
   ```bash
   dotnet restore
   ```
2. Configure `appsettings.json` (or `appsettings.Development.json`) with your own values for `Jwt:Key`, `Email` (SMTP credentials), and `ProductEtl` if desired.
3. Apply migrations:
   ```bash
   dotnet ef database update --project Repository --startup-project Web
   ```
4. Run the application:
   ```bash
   dotnet run --project Web
   ```
5. Open Swagger UI at `https://localhost:{port}/swagger` to explore and test the API.

### Testing the Full Flow

1. `POST /api/auth/register` → create a user
2. `POST /api/auth/login` → obtain a JWT
3. Authorize in Swagger/Postman using `Bearer <token>`
4. `POST /api/etl/sync` → populate the product catalog
5. `GET /api/product` → browse imported products
6. `POST /api/inventoryitem` → add a product to your inventory
7. `POST /api/expirationcheck/run` → trigger the expiration/reminder check manually

## Known Limitations

- Open Beauty Facts is crowdsourced data; not every product has complete brand, ingredient, or category information. Products missing a name or barcode are filtered out during import.
- Open Beauty Facts does not provide pricing or fixed expiration dates — these are either left blank (import) or filled in manually by the user.
- Category assignment is derived from which search query found a product; a product may receive no category if it doesn't appear in any of the configured category searches within the configured page limit.