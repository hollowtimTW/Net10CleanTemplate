# ADR 0002: PostgreSQL over SQL Server

## Status
Accepted · 2026-08-25

## Context
The legacy system is Oracle. The team needs to choose a target DB for the modernization. Two strong candidates: PostgreSQL 17 and SQL Server 2022.

## Decision
**PostgreSQL 17 is the default.**

## Rationale
| Factor | PostgreSQL | SQL Server |
|---|---|---|
| Oracle PL/SQL portability | **Excellent** (PL/pgSQL is a deliberate superset of PL/SQL syntax) | Weak (T-SQL has incompatible idioms) |
| License cost (per-proc, Enterprise) | **Free** | $25k+ per CPU |
| JSONB / GIN indexes | **Excellent** | Good |
| Taiwan hospital adoption | Strong (Chang Gung etc.) | Stronger (most medical centers) |
| Commercial support in Taiwan | Limited | Strong (Microsoft / partner ecosystem) |
| DBA ramp-up from Oracle | **Trivial** (same procedural language family) | Moderate |

## Consequences
- ✅ Lowest PL/SQL→PL/pgSQL migration cost
- ✅ Zero licensing cost
- ⚠ Production HA / TDE requires self-managed tooling or commercial add-on
- ⚠ Need to harden shared hosting for compliance (pgAudit, TLS, key rotation)

## When to flip to SQL Server
- Project requires SQL Server-specific features (Always Encrypted, SSRS, native JSON in T-SQL with `FOR JSON PATH`).
- Hospital already has MSSQL DBA team and zero Oracle expertise.
- Existing investments in MSSQL licensing make the savings irrelevant.

Switch via: replace `UseNpgsql(...)` → `UseSqlServer(...)` in `YourApp.Infrastructure.csproj`, drop `Npgsql` packages, add `Microsoft.EntityFrameworkCore.SqlServer`. No C# code change needed.