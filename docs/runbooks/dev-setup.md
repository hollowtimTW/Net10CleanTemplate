# Runbook: Dev Setup

## Prerequisites

- Windows 10 / 11 or Windows Server 2019+
- .NET 10 SDK (10.0.400 — pinned via `global.json`)
- PostgreSQL 17 (or SQL Server 2022, or SQLite for local-only)
- Git for Windows (bash)
- Visual Studio 2022 17.x / JetBrains Rider 2025.x / VS Code

## First-time setup

```bash
git clone https://github.com/hollowtimTW/Net10CleanTemplate.git
cd Net10CleanTemplate

# Install the template globally for `dotnet new net10-clean`
dotnet new install .

# Verify the template is registered
dotnet new list net10-clean
```

## Create your first project

```bash
cd D:\Work
dotnet new net10-clean --name MyCompany.Hospital --output MyCompany.Hospital
cd MyCompany.Hospital
dotnet build
```

## Local PostgreSQL

```bash
# Option 1: Docker (recommended)
docker run -d --name pg-dev -p 5432:5432 \
  -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=myapp \
  postgres:17

# Option 2: install PostgreSQL 17 locally and use pgAdmin
```

Connection string in `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "Database": "Host=localhost;Database=myapp;Username=postgres;Password=postgres"
  }
}
```

## Run tests

```bash
dotnet test
```

## Run the API

```bash
dotnet run --project Content/Api/WebApi
# Browse: https://localhost:5001/swagger
```

## Day-to-day commands

```bash
# Add a new EF Core migration
dotnet ef migrations add InitialCreate \
  --project Content/YourApp.Infrastructure \
  --startup-project Content/Api/WebApi

# Apply migrations
dotnet ef database update \
  --project Content/YourApp.Infrastructure \
  --startup-project Content/Api/WebApi

# Format code
dotnet format

# Update dependencies
dotnet list package --outdated
```

## Troubleshooting

**`global.json` mismatch** — your SDK is older than 10.0.400.
Install .NET 10 SDK from https://dot.net

**NuGet restore fails on Negotiate package** — `Microsoft.AspNetCore.Authentication.Negotiate` has known CVEs in 10.0.0. Either upgrade or pin to a non-`none` version of the CVE fix when Microsoft publishes.

**Tests can't connect to PG** — make sure the `Database` connection string points to a running instance. Use Testcontainers for CI.