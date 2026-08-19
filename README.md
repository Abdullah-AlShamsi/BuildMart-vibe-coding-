# BuildMart — Construction Tools & Materials E-Commerce Platform

A full-stack e-commerce web application for selling construction tools and materials, built with **ASP.NET Core 8 Web API**, **Entity Framework Core**, **SQL Server**, **JWT authentication**, and a **vanilla HTML/CSS/JS** frontend.

---

## 1. Project Description

BuildMart lets customers browse, search and filter a catalog of construction tools and materials, manage a shopping cart, place orders and track their status. Admins get a full dashboard to manage products, categories, orders and customers. The backend follows **Clean Architecture** (Domain → Application → Infrastructure → API) and exposes a fully documented REST API via Swagger.

**Currency:** all prices are in **Omani Rial (OMR)**, displayed with 3 decimal places (the standard OMR convention, since its subunit — the baisa — is 1/1000 of a rial rather than 1/100). This is handled entirely in the frontend's `money()` helper (`BuildMart.Frontend/js/common.js`); the database stores plain `decimal` values with no currency-specific logic.

**Product images:** the seeded catalog links to real, freely-licensed photos from [Wikimedia Commons](https://commons.wikimedia.org) (one representative photo per category, shared across that category's products) instead of placeholder icons. Each `Product.ImageUrl` / `Category.ImageUrl` is a direct Commons `Special:FilePath` link, so no image files are stored in this repo. If you replace the catalog with your own products, simply point `ImageUrl` at any image URL you have the rights to use — the frontend (`productImageHtml()` in `common.js`) renders it directly and automatically falls back to a category emoji if a URL ever fails to load.

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

## 14. Publishing to GitHub & Free Hosting (MonsterASP.NET)

### Step 1 — Push the code to GitHub

```bash
cd BuildMart
git init
git add .
git commit -m "Initial commit: BuildMart e-commerce platform"
git branch -M main
git remote add origin https://github.com/YOUR-USERNAME/YOUR-REPO-NAME.git
git push -u origin main
```

> `.gitignore` already excludes `bin/`, `obj/`, `appsettings.Development.json` and `appsettings.Production.json`, so no secrets or build artifacts get committed.

**⚠️ Make sure a `Migrations` folder exists under `BuildMart.Infrastructure`** (from `Add-Migration InitialCreate`, section 6) and is committed — the live host applies it automatically on startup via `Database.MigrateAsync()`, but only if it's actually in the repo.

---

### Step 2 — Sign up and create your site on MonsterASP.NET

1. Go to **[monsterasp.net](https://www.monsterasp.net/)** and sign up for the free **ASP.NET Core + MSSQL** plan.
2. From the [Hosting Control Panel](https://admin.monsterasp.net/), create a new **Website** (choose the ASP.NET Core template) and a new **MSSQL Database** — copy the connection string it gives you, you'll need it in Step 4.
3. **Activate WebDeploy** for your site in the Control Panel, and note down the four values it shows you:
   - `WEBSITE_NAME` (e.g. `site1234`)
   - `SERVER_COMPUTER_NAME` (e.g. `https://site1234.siteasp.net:8172`)
   - `SERVER_USERNAME`
   - `SERVER_PASSWORD`

---

### Step 3 — Add GitHub Actions secrets

This repo already includes `.github/workflows/deploy-backend-monsterasp.yml`, which builds, publishes and deploys `BuildMart.API` automatically on every push to `main` — adapted from [MonsterASP.NET's official GitHub Actions guide](https://help.monsterasp.net/books/github/page/how-to-deploy-website-via-github-actions) for this repo's multi-project solution layout.

On GitHub: **Settings → Secrets and variables → Actions → New repository secret**, and add these five secrets:

| Secret name | Value |
|---|---|
| `WEBSITE_NAME` | from Step 2.3 |
| `SERVER_COMPUTER_NAME` | from Step 2.3 |
| `SERVER_USERNAME` | from Step 2.3 |
| `SERVER_PASSWORD` | from Step 2.3 |
| `APPSETTINGS_PRODUCTION_JSON` | see below |

**For `APPSETTINGS_PRODUCTION_JSON`:** copy `BuildMart.API/appsettings.Production.json.example`, fill in real values (your MSSQL connection string from Step 2.2, a new random 32+ char `JwtSettings:SecretKey`, and your GitHub Pages URL from Step 4 under `CorsSettings:AllowedOrigins`), then paste the **entire file's content as one secret value**. The workflow writes it to `BuildMart.API/appsettings.Production.json` right before publishing — the real file is never committed to the repo.

Push to `main` (or re-run the workflow manually from the **Actions** tab) to trigger a deploy. On first successful startup, the app creates all tables and seeds categories/products/admin automatically — check the workflow's logs or your host's runtime logs for the *"Seeded default admin account..."* message.

---

### Step 4 — Frontend: deploy to GitHub Pages (free, automatic)

This repo also includes `.github/workflows/deploy-pages.yml`, which publishes `BuildMart.Frontend` to GitHub Pages on every push to `main`.

1. On GitHub: **Settings → Pages → Build and deployment → Source → GitHub Actions**.
2. Before pushing, update `BuildMart.Frontend/js/api.js`:
   ```js
   const API_BASE_URL = 'https://YOUR-SITE.siteasp.net/api'; // your live backend URL from Step 2
   ```
3. Push to `main` — it publishes to `https://YOUR-USERNAME.github.io/YOUR-REPO-NAME/`.
4. Update the `APPSETTINGS_PRODUCTION_JSON` secret's `CorsSettings:AllowedOrigins` to that exact Pages URL, then re-run the backend workflow — otherwise the browser blocks API requests with a CORS error.

### Verifying the live deployment

- Open the GitHub Pages URL → Home page should load categories/products from the live API.
- Open `https://YOUR-SITE.siteasp.net/swagger` → confirm it loads and `/api/products` returns data.
- Register a test account and place a test order end-to-end.
- Log in as `admin@buildmart.com` (whatever password you set in `APPSETTINGS_PRODUCTION_JSON`) and confirm the Admin dashboard loads.

---

### Troubleshooting

| Problem | Fix |
|---|---|
| `dotnet ef` command not found | `dotnet tool install --global dotnet-ef` |
| Frontend requests fail / CORS error | Check `API_BASE_URL` in `js/api.js` matches your API's actual port, and that the frontend's origin is listed in `CorsSettings:AllowedOrigins` |
| "Connection string not found" | Double-check `appsettings.Development.json` has a valid `DefaultConnection` |
| 401 on every request after login | Token may have expired (default: 60 minutes) — log in again |
| Migration fails with login errors | Confirm your SQL Server instance name/auth mode in the connection string; for SQL auth (not Windows auth) add `User Id=...;Password=...;` and remove `Trusted_Connection=True` |
