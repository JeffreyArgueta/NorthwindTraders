# Northwind Traders

A full-stack order management application featuring a .NET 10 Minimal API backend, a Vue 3 + Quasar frontend, and a Microsoft SQL Server database — all orchestrated with Docker Compose.

## Architecture

```
┌──────────────────────────────────────────────────┐
│                  Frontend                        │
│           Vue 3 + Quasar (Vite)                  │
│           Served via Nginx on :5173              │
└────────────────────┬─────────────────────────────┘
                     │  HTTP / REST
                     ▼
┌──────────────────────────────────────────────────┐
│                    API                           │
│     .NET 10 Minimal API (Clean Architecture)     │
│     Kestrel on :5000                             │
│  ┌─────────────┬─────────────┬────────────────┐  │
│  │ Api         │ Application │ Infrastructure │  │
│  │ (endpoints) │ (services)  │ (EF Core / DB) │  │
│  └─────────────┴─────────────┴────────────────┘  │
└────────────────────┬─────────────────────────────┘
                     │  SQL / TCP
                     ▼
┌──────────────────────────────────────────────────┐
│                  SQL Server 2025                 │
│                  on :1433                        │
└──────────────────────────────────────────────────┘
```

### Project Structure

| Path | Description |
|------|-------------|
| `docker-compose.yml` | Orchestrates all three services |
| `.env` | Environment variables (do not commit) |
| `database/` | Northwind SQL schema + seed data |
| `src/` | .NET solution (Clean Architecture) |
| `frontend/` | Vue 3 + Quasar SPA |
| `tests/` | Backend integration / unit tests |

---

## Prerequisites

- [Docker](https://docs.docker.com/get-docker/) ≥ 24
- [Docker Compose](https://docs.docker.com/compose/install/) v2 (included with Docker Desktop)
- (Optional) [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) for local development
- (Optional) [Node.js](https://nodejs.org/) ≥ 20 for local frontend development

---

## Setup

### 1. Environment Variables

The project relies on a `.env` file at the repository root. **This file is gitignored** — you must create your own.

Copy the example below into `.env` and adjust as needed:

```bash
# ── SQL Server ────────────────────────────────────
MSSQL_VERSION=2025-latest
MSSQL_SA_PASSWORD=YourStrong!Passw0rd
MSSQL_PID=Developer
MSSQL_PORT=1433
MSSQL_DB=Northwind

# ── .NET API ──────────────────────────────────────
API_PORT=5000
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=Server=sqlserver,1433;Database=${MSSQL_DB};User Id=sa;Password=${MSSQL_SA_PASSWORD};TrustServerCertificate=True;
AllowedOrigins=http://localhost:${FRONTEND_PORT}

# ── Vue 3 Frontend ────────────────────────────────
FRONTEND_PORT=5173
VITE_API_BASE_URL=http://localhost:${API_PORT}
VITE_GOOGLE_MAPS_API_KEY=your_google_maps_api_key_here
```

> **Security note:** The Google Maps API key and SA password shown above are placeholders. Replace them with your own credentials. Never commit real secrets to version control.

### 2. Start the Stack

```bash
docker compose up -d
```

This starts three containers:

| Service | Container name | Internal port | Published port |
|---------|---------------|---------------|----------------|
| SQL Server | `sqlserver` | 1433 | `${MSSQL_PORT}` (default 1433) |
| .NET API | `northwind-api` | 8080 | `${API_PORT}` (default 5000) |
| Vue Frontend | `northwind-frontend` | 80 | `${FRONTEND_PORT}` (default 5173) |

The API will wait for SQL Server to report healthy before starting.  
The frontend will wait for the API to start (basic dependency — the API may still be initializing).

> **First-time build:** The first `docker compose up` will build the API and frontend images. This can take several minutes. Subsequent starts are near-instant.

### 3. Initialize the Database

The Northwind database schema and seed data are provided in `database/northwind.sql` (~9 400 lines). Docker Compose does **not** automatically apply this script — you must run it manually once the SQL Server container is healthy.

Run the following command:

```bash
docker compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "${MSSQL_SA_PASSWORD}" \
  -C -i /dev/stdin <<-EOSQL
    CREATE DATABASE Northwind;
EOSQL

cat database/northwind.sql | docker compose exec -T sqlserver \
  /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "${MSSQL_SA_PASSWORD}" \
  -C -d Northwind
```

**Alternative — one-liner:**

```bash
cat database/northwind.sql | docker compose exec -T sqlserver \
  /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "${MSSQL_SA_PASSWORD}" -C
```

> The `northwind.sql` script already contains `CREATE DATABASE` and `USE Northwind` statements, so piping it directly works. The `-C` flag trusts the server certificate (required for SQL Server 2022+).

**Verify the database was created:**

```bash
docker compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "${MSSQL_SA_PASSWORD}" \
  -C -Q "SELECT name FROM sys.databases"
```

### 4. Access the Application

| Service | URL |
|---------|-----|
| Frontend (Vue 3) | [http://localhost:5173](http://localhost:5173) |
| API (Swagger UI) | [http://localhost:5000/swagger](http://localhost:5000/swagger) |
| API (health) | [http://localhost:5000/api/orders](http://localhost:5000/api/orders) |
| SQL Server | `localhost:1433` (via any SQL client) |

---

## Managing the Stack

### View logs

```bash
# All services
docker compose logs -f

# Specific service
docker compose logs -f api
docker compose logs -f frontend
docker compose logs -f sqlserver
```

### Stop / restart

```bash
docker compose down        # Stop and remove containers (data persists in volume)
docker compose down -v     # ⚠️ Also delete the mssql-data volume (destroys all data)
docker compose restart     # Restart all services
```

### Rebuild after code changes

```bash
docker compose build --no-cache api     # Rebuild the API image
docker compose build --no-cache frontend # Rebuild the frontend image
docker compose up -d                    # Start with rebuilt images
```

### Reset everything

```bash
docker compose down -v
docker compose up -d
# Then re-run the database initialization step
```

---

## Development (Without Docker)

### Backend (.NET)

```bash
cd src
dotnet restore NorthwindTraders.slnx
dotnet build NorthwindTraders.slnx

# Run the API (uses appsettings.Development.json by default)
dotnet run --project NorthwindTraders.Api

# Run tests
dotnet test NorthwindTraders.slnx
```

When running locally, the API connects to `localhost,1433` (see `appsettings.Development.json`). Make sure SQL Server is running — either via Docker Compose (`docker compose up sqlserver -d`) or a local instance.

### Frontend (Vue 3)

```bash
cd frontend
npm install
npm run dev          # Start Vite dev server (default :5173)
npm run test:unit    # Run unit tests
npm run build        # Production build
npm run lint         # Lint and format
```

The frontend expects `VITE_API_BASE_URL` to point to the running API. By default it uses `http://localhost:5000`.

---

## API Endpoints

All endpoints follow the pattern `/api/{resource}` and support standard CRUD operations with pagination.

| Resource | Endpoints | Notes |
|----------|-----------|-------|
| **Orders** | `GET/POST /api/orders`, `GET/PUT/DELETE /api/orders/{id}` | |
| **Order Details** | `GET/POST /api/orders/{orderId}/details`, `GET/PUT/DELETE /api/orders/{orderId}/details/{productId}` | |
| **Customers** | `GET/POST /api/customers`, `GET/PUT/DELETE /api/customers/{id}` | `id` is string (nchar(5)) |
| **Employees** | `GET/POST /api/employees`, `GET/PUT/DELETE /api/employees/{id}` | |
| **Products** | `GET/POST /api/products`, `GET/PUT/DELETE /api/products/{id}` | |
| **Shippers** | `GET/POST /api/shippers`, `GET/PUT/DELETE /api/shippers/{id}` | |
| **Suppliers** | `GET/POST /api/suppliers`, `GET/PUT/DELETE /api/suppliers/{id}` | |
| **Categories** | `GET /api/categories`, `GET /api/categories/{id}` | Read-only |
| **Regions** | `GET /api/regions`, `GET /api/regions/{id}` | Read-only |

Pagination is supported via `?page=1&pageSize=20` query parameters (max `pageSize` is 100).

---

## Technology Stack

### Backend
- **.NET 10** with Minimal API (ASP.NET Core)
- **Entity Framework Core 10** — data access
- **FluentValidation** — input validation
- **Swagger / OpenAPI** — API documentation
- **Clean Architecture** — Domain / Application / Infrastructure / Api layers

### Frontend
- **Vue 3** — Composition API with `<script setup>`
- **Quasar 2** — UI component framework
- **Pinia** — state management
- **Vue Router** — client-side routing
- **Vite** — build tool and dev server
- **Axios** — HTTP client
- **jspdf + jspdf-autotable** — PDF report generation
- **Google Maps Address Validation API** — address lookup

### Infrastructure
- **Docker Compose** — container orchestration
- **SQL Server 2025** — relational database
- **Nginx** — frontend reverse proxy (production)
