# Skincare Inventory App

A skincare inventory management system built with ASP.NET Core. Users can browse a product catalog, track owned products, maintain a wishlist, log ingredient allergies/reactions, and receive email reminders before products expire. Product data is populated automatically via a scheduled ETL pipeline that pulls from the Open Beauty Facts public API. A lightweight HTML/Bootstrap frontend is included for demoing the full flow without a separate testing tool.

## Tech Stack

- **Backend:** ASP.NET Core Web API (.NET 10)
- **Frontend:** Single-page HTML + Bootstrap 5 + vanilla JavaScript (served as a static file from the API itself)
- **Database:** SQLite (via Entity Framework Core)
- **Authentication:** Custom JWT bearer authentication (no ASP.NET Core Identity)
- **Scheduling:** Quartz.NET
- **Email:** MailKit (SMTP)
- **External API:** Open Beauty Facts (`world.openbeautyfacts.org`)

## Architecture

The solution is organized into four layered projects:

```
Domain      — entities, enums, DTOs, config, and pure domain services (no external dependencies)
Repository  — EF Core DbContext, generic repository implementation
Service     — business logic, use-case orchestration, external API clients, background jobs
Web         — controllers, request/response DTOs, mappers, static frontend, composition root (Program.cs)
```

Dependencies flow inward: `Web` depends on `Service` and `Repository`; `Service` and `Repository` both depend on `Domain`. `Domain` has no dependencies on any other project.

Each domain area (`Product`, `InventoryItem`, `WishlistItem`, `Category`, `Ingredient`, `IngredientReaction`) follows the same pattern across layers: a `Service` interface + implementation, a `Web` mapper that translates between the domain model and request/response records, and a thin controller.

## Data Model

| Entity | Description |
|---|---|
| `User` | Application user; plain entity, not tied to ASP.NET Core Identity |
| `Product` | Shared catalog item (name, brand, barcode, image, price, ingredients); populated via manual entry or the ETL pipeline |
| `Category` | Fixed taxonomy (Skincare, Bodycare, Cosmetics, Sun Care); seeded at startup |
| `ProductCategory` | Many-to-many join between `Product` and `Category` (composite key) |
| `Ingredient` | Shared ingredient reference data, parsed from product ingredient lists during ETL |
| `InventoryItem` | A specific product a specific user owns, with purchase/opened/expiration dates, PAO tracking, rating, comment, and status |
| `WishlistItem` | A product a specific user wants, with its own status lifecycle |
| `IngredientReaction` | **Ternary relation** — links `User` + `Ingredient`, with an optional `Product` link, recording a reaction type and severity |
| `EtlSyncLog` | Audit record of each ETL sync run (success/failure, counts, timestamps) |

### Ternary Relation

`IngredientReaction` connects `User`, `Ingredient`, and (optionally) `Product` in a single relation, each with its own foreign key, plus attributes of its own (`ReactionType`, `ReactionSeverity`, `Note`). `ProductId` is nullable to support two distinct use cases from the same entity:

- **A general allergy declaration** (`ProductId = null`) — "I'm allergic to Niacinamide," independent of any specific product.
- **A reaction logged against a specific owned product** (`ProductId` set) — "I used this product and reacted to this ingredient in it," exercising the full three-way relation between `User`, `Product`, and `Ingredient`.

This powers a business-logic feature: before a user adds a product to their inventory or wishlist, the system checks whether that product contains any ingredient the user has previously reacted to (`GetConflictingIngredientsAsync`), warning them before they proceed.

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
Active/Opened → Expired   (system-driven, via the scheduled expiration check job)
```

`Finished`, `Discarded`, and `Expired` are terminal — no transition exists back out of them. Each transition is exposed as its own service method (`OpenProductAsync`, `FinishProductAsync`, `DiscardProductAsync`) with its own guard clause, rather than a single generic status-setter, so illegal transitions are rejected explicitly.

### Ingredient Conflict Warnings

Before a product is added to a user's inventory or wishlist, the frontend checks `GET /api/ingredientreaction/conflicts/{productId}`, which cross-references the product's linked ingredients against the user's logged reactions. If any match, the user is warned before the action proceeds — a direct, practical use of the ternary relation.

## Integrations

### ETL Pipeline (Extract, Transform, Load)

**Source:** Open Beauty Facts public search API (`/api/v2/search`), queried per category tag (`en:face-care`, `en:body-care`, `en:cosmetics`, `en:sun-care`).

- **Extract:** `ExternalProductApi` calls the search endpoint, paginating through results per configured category.
- **Transform:** `ExternalProductTransformer` maps the raw API response into `Product` entities, and parses `ingredients_text` into individual ingredient names.
- **Load:** `EtlSyncService` upserts products by barcode, links parsed ingredients via the `Ingredient` many-to-many relation, and tags each product with a `Category` based on the search that found it.

Every run is recorded in `EtlSyncLog` (start/end time, success flag, error message if any, counts of products imported/updated/skipped).

### External API Integration

The same `IExternalProductApi` abstraction used by the ETL job is the external API integration point — Open Beauty Facts is a live, publicly documented, third-party REST API queried at runtime, not a static dataset.

### Scheduling (Quartz.NET)

Both recurring background tasks are scheduled through Quartz rather than manual timer loops:

| Job | Schedule | Purpose |
|---|---|---|
| `QuartzEtlSync` | Every 24 hours | Runs the ETL sync described above |
| `QuartzExpirationCheck` | Weekly (cron: `0 0 3 ? * MON`, every Monday at 3 AM) | Scans inventory for expiring/expired items |

Jobs are registered with `AddJob<T>` and their triggers with `AddTrigger`, using either a simple interval schedule or a cron schedule depending on the job. Quartz's hosted service (`AddQuartzHostedService`) runs these on the configured intervals for the lifetime of the application. Both jobs can also be triggered manually via API endpoints for testing/demo purposes (see below).

### Asynchronous Queue-Based Processing

Email sending is decoupled from expiration detection using an in-process, channel-based message queue (`System.Threading.Channels`):

- `IEmailQueue` defines `EnqueueAsync` (producer side) and `DequeueAllAsync` (consumer side), backed by an unbounded `Channel<EmailMessage>`.
- `ExpirationCheckService` (the producer) enqueues an `EmailMessage` for each item that needs a reminder, rather than sending the email synchronously itself.
- `EmailQueueConsumer`, a separate long-running `BackgroundService`, continuously reads from the queue (`await foreach` over `DequeueAllAsync`) and dispatches each message to `IEmailSender` for actual delivery.

This separates "detecting that a reminder is needed" from "delivering the email," communicating only through queued messages — the producer and consumer run independently and are not directly aware of each other.

### Email Integration

`IEmailSender` (implemented via MailKit/SMTP) performs the actual sending, invoked exclusively by `EmailQueueConsumer` as described above. When an item's effective expiration date has passed, `ExpirationCheckService` also transitions its status to `Expired`; when it's within 7 days of expiring and no reminder has been sent yet, a message is queued and `ReminderSent` is flagged to prevent duplicate notifications.

### Authentication

JWT bearer authentication is implemented from scratch against the application's own `User` entity — no ASP.NET Core Identity. Passwords are hashed using `Microsoft.AspNetCore.Identity`'s standalone `PasswordHasher<T>` utility, used purely as a hashing algorithm without pulling in Identity's broader architecture (`IdentityUser`, `IdentityDbContext`, etc.).

```
POST /api/auth/register
POST /api/auth/login
```

A successful login returns a JWT containing the user's ID as a `ClaimTypes.NameIdentifier` claim, which `ICurrentUserService` reads on every authenticated request to scope data to the logged-in user.

## Frontend

A single-page frontend (`Web/wwwroot/index.html`) is served as a static file from the same origin as the API, avoiding any CORS configuration. It covers:

- **Login / Register**
- **Browse Products** — server-side paged (`/api/product/paged`, 9 per page) when browsing, client-side paged search results when a search term is entered (`/api/product?name=`)
- **My Inventory** — items grouped into sections by status (Active, Opened, Finished, Discarded, Expired), each independently paginated; edit comment/rating; transition status (Open/Finish/Discard); log a reaction to a specific product
- **Wishlist** — items grouped by status, with actions to move an item to inventory or remove it
- **My Allergies** — a list of logged ingredient reactions (general or product-specific), with add/remove
- Ingredient conflict warnings shown before adding a product to inventory or wishlist

## Getting Started

### Prerequisites
- .NET 10 SDK
- EF Core CLI tools: `dotnet tool install --global dotnet-ef`
- SMTP credentials for the email account used to send reminders (e.g. a Gmail account with an App Password)

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
5. Open `https://localhost:{port}/index.html` in a browser to use the frontend.

### Testing

The frontend covers the primary user flow end to end. For direct API testing, Postman is used rather than Swagger:

1. `POST /api/auth/register` — create a user
2. `POST /api/auth/login` — returns a JWT in the response body
3. On subsequent requests, set **Authorization → Bearer Token** in Postman and paste the token
4. `POST /api/etl/sync` — manually trigger the ETL pipeline immediately, without waiting for the daily Quartz schedule
5. `GET /api/product` or `GET /api/product/paged` — browse imported products
6. `POST /api/inventoryitem` — add a product to your inventory (requires a valid `productId`)
7. `POST /api/expirationcheck/run` — manually trigger the expiration/reminder check, without waiting for the weekly Quartz schedule

Requests with a body must have their Content-Type set to JSON in Postman (the "raw" body type dropdown must say **JSON**, not **Text**), or the API will reject the request with `415 Unsupported Media Type`.

## Known Limitations

- Open Beauty Facts is crowdsourced data; not every product has complete brand, ingredient, or category information. Products missing a name or barcode are filtered out during import.
- Open Beauty Facts does not provide pricing or fixed expiration dates — these are either left blank on import or filled in manually by the user.
- Category and ingredient assignment are derived from which search query found a product; a product may receive no category or no ingredients if it doesn't appear in any configured category search, or if its ingredient text couldn't be parsed cleanly.
- The email queue is in-process (`System.Threading.Channels`), so queued messages are lost if the application restarts before they're processed. A production deployment would use a persistent broker (e.g. RabbitMQ) instead.
- `Category` does not expose create/update/delete endpoints, since it is treated as a fixed, seeded taxonomy rather than user-editable data.