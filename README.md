# EGit .NET 10 Clean Template

A reusable [`dotnet new`](https://learn.microsoft.com/dotnet/core/tools/dotnet-new) template that scaffolds a complete .NET 10 LTS solution with Clean Architecture, EF Core 10 + Dapper dual-track, PostgreSQL default (SQL Server swap-ready), MediatR, FluentValidation, Serilog, optional Razor Pages / MVC / Web API frontends.

> **Short name:** `net10-clean`
> **Author:** Tim @ EGit (hollowtimTW)
> **Target framework:** .NET 10.0 (LTS) — C# 14

---

## ✨ What's included

| Layer | Projects | Description |
|---|---|---|
| **Domain** | `YourApp.Domain` | Pure POCO aggregates, value objects, domain events, Result pattern. Zero external deps. |
| **Application** | `YourApp.Application` | Abstractions (`IUnitOfWork`, `ICurrentUser`, `IAuditWriter`, `IDateTime`), MediatR + FluentValidation + Mapperly. |
| **Infrastructure** | `YourApp.Infrastructure` | `EfUnitOfWork` + `NpgsqlConnectionFactory` + `NpgsqlBulkWriter` (COPY) + `AuditSaveChangesInterceptor`. |
| **Hosting** | `YourApp.Shared.Hosting` | One-line Serilog / ProblemDetails / Swagger / HealthCheck setup. |
| **Identity** | `YourApp.Identity` | JWT + Cookie + Windows Auth (Negotiate), `IUserDirectory`, `IJwtIssuer`, `IBreakGlassService`. |
| **AuditLog** | `YourApp.AuditLog` | Bounded channel; Web pushes, AuditLog.Writer worker drains to sink. |
| **Notification** | `YourApp.Notification` | Email / SMS / Push gateway with channel decoupling. |
| **FileStorage** | `YourApp.FileStorage` | Local disk default; pluggable for NAS / S3 / Azure Blob. |
| **API host** | `YourApp.WebApi` | Minimal API endpoints + ProblemDetails + HealthCheck. |
| **MVC host** | `YourApp.Mvc.Web` | Controllers + Views + ViewModels + Highcharts dashboard. |
| **Razor Pages host** | `YourApp.Razor.Web` | Pages + PageModels + Highcharts dashboard. |
| **Workers** | `YourApp.AuditLog.Writer` / `YourApp.Notification.Dispatcher` | Standalone Worker SDK services. |

**17 csproj, ~80 source files, 60+ NuGet packages pinned in CPM.**

---

## 🚀 Quick Start

### Install the template locally
```bash
git clone https://github.com/hollowtimTW/Net10CleanTemplate.git
cd Net10CleanTemplate
dotnet new install .
```

### Create a new project
```bash
dotnet new net10-clean --name MyCompany.Hospital --output ~/work/MyCompany.Hospital
```

### Build + run
```bash
cd ~/work/MyCompany.Hospital
dotnet build                           # 0 warnings, 0 errors
dotnet run --project Content/Api/WebApi
# → https://localhost:5001/swagger
```

---

## 🧱 Architecture diagram

```
┌──────────────────────────────────────────┐
│  YourApp.WebApi  /  Mvc.Web  /  Razor.Web │
│  (one or more, by --frontend flag)      │
└────────────┬─────────────────────────────┘
             │
   ┌─────────▼──────────┐    ┌─────────────┐
   │  YourApp.Application│    │ YourApp.    │
   │  (MediatR handlers, │◄───┤ Domain      │
   │   validators)        │    │ (pure POCO) │
   └─────────┬───────────┘    └─────────────┘
             │
   ┌─────────▼────────────────────────────┐
   │  YourApp.Infrastructure              │
   │  • EfUnitOfWork + Dapper (dual-track) │
   │  • AuditSaveChangesInterceptor         │
   │  • NpgsqlConnectionFactory + COPY    │
   └──────────────────────────────────────┘
             │
   ┌─────────▼────────────────────────────┐
   │  PostgreSQL 17 (default)             │
   │  • snake_case tables                  │
   │  • JSONB columns + GIN indexes       │
   │  • window functions                  │
   └──────────────────────────────────────┘
```

---

## 🎛 Template parameters

| Flag | Choices | Default | Description |
|---|---|---|---|
| `--frontend` | `none`, `mvc`, `razor`, `all` | `razor` | Which web frontend to scaffold. |
| `--database` | `postgresql`, `sqlserver` | `postgresql` | DB provider (PostgreSQL by default for Oracle portability). |
| `--auth` | `cookie+jwt`, `windows`, `all` | `cookie+jwt` | Authentication mode. |
| `--includeIdentity` | bool | `true` | Include Identity module. |
| `--includeAuditLog` | bool | `true` | Include AuditLog module. |
| `--includeNotification` | bool | `true` | Include Notification module. |
| `--includeFileStorage` | bool | `true` | Include FileStorage module. |
| `--useMediatR` | bool | `true` | Use MediatR for CQRS. |
| `--includeTests` | bool | `true` | Generate test projects. |

---

## 🧪 Adding a new subsystem

1. Copy `Content/YourApp.Domain/` → `Content/MyFeature.Domain/`
2. Copy `Content/YourApp.Application/` → `Content/MyFeature.Application/`
3. Copy `Content/YourApp.Infrastructure/` → `Content/MyFeature.Infrastructure/`
4. Add the 3 csprojs to your `.sln`
5. Add ProjectReferences from your new projects to the shared `YourApp.*` projects

See `docs/architecture/` for the full ADR set.

---

## 📦 Pinned package versions

All packages are managed via **Central Package Management** (`Directory.Packages.props`).

| Package | Version | Note |
|---|---|---|
| `Microsoft.EntityFrameworkCore` | 10.0.1 | |
| `Microsoft.EntityFrameworkCore.Design` | 10.0.1 | M |
| `Npgsql` | 10.0.0 | |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | 10.0.0 | |
| `EFCore.NamingConventions` | 10.0.1 | |
| `Dapper` | 2.1.66 | |
| `MediatR` | 14.2.0 | Last MIT-licensed version (15+ is commercial). |
| `FluentValidation` | 11.11.0 | |
| `Riok.Mapperly` | 4.3.1 | Source-gen mapper (no reflection). |
| `Serilog` | 4.4.0 | |
| `Serilog.AspNetCore` | 9.0.0 | |
| `Serilog.Sinks.Seq` | 9.0.0 | |
| `Swashbuckle.AspNetCore` | 9.0.3 | |
| `Hangfire.AspNetCore` | 1.8.21 | Optional. |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 10.0.0 | |
| `Microsoft.AspNetCore.Authentication.Negotiate` | 10.0.0 | ⚠ Has known CVE — patch on upgrade. |
| `xunit` | 2.9.3 | |
| `AwesomeAssertions` | 9.0.0 | |
| `Testcontainers.PostgreSql` | 4.8.0 | |

---

## 🩺 Hospital / regulated-environment notes

This template was originally built to support hospital-grade ASP.NET Core modernization. The following practices are wired in by default:

- ✅ **Result pattern** (`Result<T>`) instead of exceptions for expected failures
- ✅ **Audit interceptor** automatically records every CRUD to structured log
- ✅ **Channel-based audit/notification decoupling** — request latency ≠ audit write latency
- ✅ **Dapper + EF Core dual-track** — OLTP goes EF, complex reports / window functions / stored-procedure calls go Dapper
- ✅ **PostgreSQL JSONB-ready** via `EnableDynamicJson()` on NpgsqlDataSource
- ✅ **Bulk writer** using PostgreSQL `COPY ... FROM STDIN` (~30× faster than SaveChanges for large inserts)
- ✅ **Cookie + JWT + Windows Auth** out of the box (Cookie for web, JWT for API, Negotiate for AD SSO)

When swapping to MSSQL, swap `UseNpgsql` → `UseSqlServer` and remove `Npgsql`-specific packages. Everything else works as-is.

---

## 📁 Repository layout

```
Net10CleanTemplate/
├── .template.config/template.json    ← dotnet new metadata
├── Content/                          ← what gets scaffolded into your new project
│   ├── YourApp.Domain/
│   ├── YourApp.Application/
│   ├── YourApp.Infrastructure/
│   ├── YourApp.Shared.Hosting/
│   ├── YourApp.Identity/
│   ├── YourApp.AuditLog/
│   ├── YourApp.Notification/
│   ├── YourApp.FileStorage/
│   ├── Api/WebApi/                   ← YourApp.WebApi
│   ├── Mvc/Mvc.Web/                  ← YourApp.Mvc.Web
│   ├── Razor/Razor.Web/              ← YourApp.Razor.Web
│   └── Workers/
│       ├── AuditLog.Writer/
│       └── Notification.Dispatcher/
├── .dev-tools/
│   └── samples/                      ← AcmeApp sample (proves template works)
├── deploy/                           ← IIS + Docker templates
├── docs/                             ← ADRs + security checklist + runbook
├── global.json
├── Directory.Build.props
├── Directory.Packages.props
├── .gitignore
└── README.md
```

---

## 🤝 Contributing

PRs welcome. Run `dotnet build` + `dotnet test` before submitting.

---

## 📝 License

MIT — see [LICENSE](LICENSE).