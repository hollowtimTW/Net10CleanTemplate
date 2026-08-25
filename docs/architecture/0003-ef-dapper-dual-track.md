# ADR 0003: EF Core 10 + Dapper Dual-Track

## Status
Accepted · 2026-08-25

## Context
Hospital reports involve complex SQL (window functions, recursive CTEs, GROUPING SETS). Pure EF Core struggles with both performance and translator limitations. Pure Dapper loses aggregate lifecycle management.

## Decision
**Use EF Core 10 for OLTP writes and entity lifecycle. Use Dapper 2.x for read-only queries (reports, dashboards, window functions).**

## How
- Both share the same `DbConnection` via `EfUnitOfWork.OpenConnectionAsync()`
- Both can run inside the same transaction (EF SaveChanges + Dapper SELECT atomic)
- EF Core owns entity change tracking + migrations; Dapper owns raw SQL

## Consequences
- ✅ Reports stay fast and predictable
- ✅ Stored procedures from Oracle migration land as-is in PG (called via Dapper)
- ⚠ Dapper queries bypass EF tracking → no auto-cache → must project to DTOs explicitly
- ⚠ Two syntaxes to maintain (LINQ + raw SQL)

## When to drop Dapper
- Pure CRUD app with no reports → EF Core alone is enough
- Microservice with no read/write contention

## References
- See `Content/YourApp.Infrastructure/Persistence/EfUnitOfWork.cs`
- See `Content/YourApp.Infrastructure/Persistence/NpgsqlBulkWriter.cs` (COPY protocol)