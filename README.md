# BuildMart — Construction Tools & Materials E-Commerce Platform

A full-stack e-commerce web application for selling construction tools and materials, built with **ASP.NET Core 8 Web API**, **Entity Framework Core**, **SQL Server**, **JWT authentication**, and a **vanilla HTML/CSS/JS** frontend.

---

## 1. Project Description

BuildMart lets customers browse, search and filter a catalog of construction tools and materials, manage a shopping cart, place orders and track their status. Admins get a full dashboard to manage products, categories, orders and customers. The backend follows **Clean Architecture** (Domain → Application → Infrastructure → API) and exposes a fully documented REST API via Swagger.

---

## 2. Technologies

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core 8 Web API |
| Database | SQL Server (SQL Server Express works fine for local dev) |
| ORM | Entity Framework Core 8 (Code-First + Migrations) |
| Auth | ASP.NET Core Identity + JWT Bearer tokens |
| Validation | FluentValidation |
| API Docs | Swagger / OpenAPI (Swashbuckle) |
| Frontend | HTML5, CSS3, vanilla JavaScript (fetch API) |
| Architecture | Clean Architecture (Domain / Application / Infrastructure / API) |

---

## 3. Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- SQL Server (LocalDB, Express, or full SQL Server) — a connection you can point to
- A modern browser
- (Optional) A static file server for the frontend, e.g. VS Code's "Live Server" extension, or just open the HTML files directly in a browser

---

## 4. Installation

```bash
# 1. Extract the project archive, then from the solution root:
cd BuildMart

# 2. Restore all NuGet packages
dotnet restore
```

### Required NuGet packages (already declared in the .csproj files — restored automatically)

**BuildMart.Domain**
- `Microsoft.Extensions.Identity.Stores`

**BuildMart.Application**
- `FluentValidation`
- `FluentValidation.DependencyInjectionExtensions`
- `AutoMapper` / `AutoMapper.Extensions.Microsoft.DependencyInjection` *(installed for future use; current services map manually for reliability)*

**BuildMart.Infrastructure**
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.EntityFrameworkCore.Design`
- `Microsoft.EntityFrameworkCore.Tools`
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
- `System.IdentityModel.Tokens.Jwt`
- `Microsoft.Extensions.DependencyInjection`
- `Microsoft.Extensions.Configuration.Abstractions`
- `Microsoft.Extensions.Logging.Abstractions`

**BuildMart.API**
- `Microsoft.AspNetCore.Authentication.JwtBearer`
- `Swashbuckle.AspNetCore`
- `Microsoft.EntityFrameworkCore.Design`

---

## 5. Database Setup

1. Open `BuildMart.API/appsettings.Development.json` and update `ConnectionStrings:DefaultConnection` to point at your SQL Server instance:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.\\SQLEXPRESS;Database=BuildMartDb_Dev;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

2. Update the JWT secret for local dev (must be at least 32 characters):

```json
"JwtSettings": {
  "SecretKey": "Your-Own-Long-Random-Development-Secret-Key!"
}
```

> ⚠️ **Never commit real secrets.** For anything beyond local development, use `dotnet user-secrets` or environment variables instead of `appsettings.json`.

---

## 6. Migration Commands

> Using Visual Studio? Skip to section 7 — the Package Manager Console commands there don't require installing anything extra.

```bash
# From the solution root (BuildMart/)

# Create the initial migration
dotnet ef migrations add InitialCreate --project BuildMart.Infrastructure --startup-project BuildMart.API

# Apply it to the database (creates tables + seeds categories/products via HasData)
dotnet ef database update --project BuildMart.Infrastructure --startup-project BuildMart.API
```

The Identity roles (`Admin`, `Customer`) and the default admin account are seeded automatically **at application startup** (see `Program.cs` → `IdentitySeeder.SeedAsync`), not through a migration — this is required because the password must be hashed through `UserManager`.

If you'd rather let the app apply migrations for you, `Program.cs` already calls `db.Database.MigrateAsync()` on startup, so simply running the API after `dotnet ef migrations add` (without `database update`) also works.

---

## 7. How to Run the Backend

### Option A — Visual Studio (recommended if you have it installed)

1. Double-click `BuildMart.sln` to open the whole solution.
2. In **Solution Explorer**, right-click **BuildMart.API** → **Set as Startup Project**.
3. Open `BuildMart.API/appsettings.Development.json` and confirm `ConnectionStrings:DefaultConnection` points at a SQL Server instance you actually have (see section 5 above).
4. Create the database via the **Package Manager Console** (`Tools → NuGet Package Manager → Package Manager Console`) — this uses the `Microsoft.EntityFrameworkCore.Tools` package already referenced in the project, so **you do NOT need the `dotnet-ef` CLI tool for this path**:
   ```powershell
   Default project: BuildMart.Infrastructure

   Add-Migration InitialCreate -Project BuildMart.Infrastructure -StartupProject BuildMart.API
   Update-Database -Project BuildMart.Infrastructure -StartupProject BuildMart.API
   ```
5. Press **F5** (or the green ▶ "https" button in the toolbar). Visual Studio will build, launch Kestrel on the fixed ports below (set in `Properties/launchSettings.json`), and automatically open Swagger in your browser:
   - `https://localhost:7099/swagger`
   - `http://localhost:5099/swagger`

   These are the exact ports `BuildMart.Frontend/js/api.js` already points to (`API_BASE_URL = 'https://localhost:7099/api'`), so **no extra port matching is needed** with this path.
6. On first run, Visual Studio may prompt to trust the local HTTPS dev certificate — click **Yes**.

### Option B — Command line

```bash
cd BuildMart.API
dotnet run
```

This uses the same `launchSettings.json` profiles as Visual Studio, so it will also serve on `https://localhost:7099` / `http://localhost:5099`. If you prefer the CLI for migrations too, install the EF tool once:
```bash
dotnet tool install --global dotnet-ef --version 8.0.8
# close and reopen your terminal so PATH updates, then:
dotnet ef migrations add InitialCreate --project BuildMart.Infrastructure --startup-project BuildMart.API
dotnet ef database update --project BuildMart.Infrastructure --startup-project BuildMart.API
```

**⚠️ Important:** if you change the ports (in `launchSettings.json` or via a different `dotnet run` profile), update `API_BASE_URL` in `BuildMart.Frontend/js/api.js` and `CorsSettings:AllowedOrigins` in `appsettings.json` to match.

---

## 8. How to Run the Frontend

The frontend is plain HTML/CSS/JS — no build step required.

**Option A — VS Code Live Server (recommended)**
1. Open the `BuildMart.Frontend` folder in VS Code.
2. Right-click `index.html` → "Open with Live Server".

**Option B — Any static server**
```bash
cd BuildMart.Frontend
python -m http.server 5500
# then open http://127.0.0.1:5500
```

Make sure the port you use here is listed in `CorsSettings:AllowedOrigins` in the API's `appsettings.json`.

---

## 9. Swagger URL

Once the API is running in Development mode:

```
https://localhost:7099/swagger
```

Swagger UI lets you try every endpoint directly, including authenticated ones — click **Authorize**, paste your JWT token (just the token, no `Bearer ` prefix), and all subsequent requests will include it.

---

## 10. Default Admin Account

| Field | Value |
|---|---|
| Email | `admin@buildmart.com` |
| Password | `Admin123!` |

> ⚠️ **Development credentials only.** These are seeded automatically on first run from `appsettings.json` (`SeedAdmin` section). **Change them (or override via user-secrets/environment variables) before deploying anywhere beyond your own machine.**

Customer accounts are created normally through **Register** — there is no seeded customer account by default.

---

## 11. Example API Requests

**Register**
```http
POST /api/auth/register
Content-Type: application/json

{
  "fullName": "Ahmed Al-Balushi",
  "email": "ahmed@example.com",
  "password": "Passw0rd!",
  "confirmPassword": "Passw0rd!",
  "phoneNumber": "+96891234567"
}
```

**Login**
```http
POST /api/auth/login
Content-Type: application/json

{ "email": "admin@buildmart.com", "password": "Admin123!" }
```

**Search & filter products**
```http
GET /api/products?search=drill&categoryId=1&minPrice=20&maxPrice=100&sortBy=price_asc&page=1&pageSize=12
```

**Add to cart** (requires `Authorization: Bearer <token>`)
```http
POST /api/cart/items
Content-Type: application/json
Authorization: Bearer eyJhbGciOi...

{ "productId": 1, "quantity": 2 }
```

**Checkout**
```http
POST /api/orders
Authorization: Bearer eyJhbGciOi...

{
  "shippingAddress": "123 Al Nahda St, Muscat",
  "phoneNumber": "+96891234567",
  "paymentMethod": "CashOnDelivery"
}
```

**Admin: update order status**
```http
PUT /api/orders/1/status
Authorization: Bearer <admin-token>

{ "orderStatus": "Shipped" }
```

---

## 12. Testing Checklist

- [ ] `dotnet restore` completes with no errors
- [ ] `dotnet ef migrations add InitialCreate` generates a migration with all 7 tables + Identity tables
- [ ] `dotnet ef database update` creates the database and seeds 8 categories + 30 products
- [ ] API starts and seeds the admin account (check console log: *"Seeded default admin account..."*)
- [ ] `GET /api/products` returns paginated results
- [ ] `GET /api/products?search=drill` returns matching products only
- [ ] `GET /api/products?categoryId=1&sortBy=price_asc` filters and sorts correctly
- [ ] `POST /api/auth/register` creates a Customer account and returns a JWT
- [ ] `POST /api/auth/login` with admin credentials returns a JWT with the `Admin` role claim
- [ ] Customer-only endpoints (`/api/cart/*`) reject Admin tokens with 403 and reject missing tokens with 401
- [ ] Admin-only endpoints (`POST /api/products`, `PUT /api/orders/{id}/status`, etc.) reject Customer tokens with 403
- [ ] Adding more items to cart than available stock returns 400 with a clear message
- [ ] Placing an order deducts stock and empties the cart
- [ ] Cancelling an order (Admin) restocks the items
- [ ] Frontend: browse → filter → add to cart → checkout → view order flow works end-to-end
- [ ] Frontend: Admin dashboard shows correct stats, and CRUD works for products/categories
- [ ] Swagger UI loads at `/swagger` and "Authorize" works with a pasted JWT

---

## 13. Final Verification / Architecture Notes

- **Clean Architecture**: `Domain` has zero dependencies on EF Core or ASP.NET Core (only `Microsoft.Extensions.Identity.Stores` for the `IdentityUser` base class). `Application` depends only on `Domain`. `Infrastructure` implements `Application`'s interfaces using EF Core. `API` wires everything together.
- **No repository layer**: Application interfaces (`IProductService`, `IOrderService`, etc.) are implemented directly by Infrastructure services against `ApplicationDbContext`. This was a deliberate simplification over a full Repository+UnitOfWork layer — it keeps the codebase smaller while still keeping Application fully decoupled from EF Core (it only sees interfaces).
- **Global error handling**: every domain exception (`NotFoundException`, `BadRequestException`, `ConflictException`, `ForbiddenException`, `UnauthorizedAppException`) is mapped to the correct HTTP status code by `ExceptionHandlingMiddleware`, with a uniform `{ success, message, data, errors }` response envelope.
- **Validation**: FluentValidation validators run automatically via a global `ValidationFilter` action filter — no manual validation calls needed in controllers.
- **Stock integrity**: stock is checked when adding to cart, re-validated at checkout, deducted transactionally when an order is created, and restored if an order is cancelled.
- **Security**: passwords hashed via ASP.NET Core Identity, JWT-signed with HMAC-SHA256, role-based `[Authorize(Roles = "Admin")]` on every admin endpoint, CORS restricted to configured origins, no secrets committed to source (`.gitignore` excludes environment-specific `appsettings.*.json`).

### Troubleshooting

| Problem | Fix |
|---|---|
| `dotnet ef` command not found | `dotnet tool install --global dotnet-ef` |
| Frontend requests fail / CORS error | Check `API_BASE_URL` in `js/api.js` matches your API's actual port, and that the frontend's origin is listed in `CorsSettings:AllowedOrigins` |
| "Connection string not found" | Double-check `appsettings.Development.json` has a valid `DefaultConnection` |
| 401 on every request after login | Token may have expired (default: 60 minutes) — log in again |
| Migration fails with login errors | Confirm your SQL Server instance name/auth mode in the connection string; for SQL auth (not Windows auth) add `User Id=...;Password=...;` and remove `Trusted_Connection=True` |
