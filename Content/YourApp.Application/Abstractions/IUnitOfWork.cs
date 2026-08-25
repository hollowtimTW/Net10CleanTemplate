using System.Data.Common;

namespace YourApp.Application.Abstractions;

/// <summary>
/// Unit of Work abstraction. EF Core SaveChanges + Dapper read access share the same
/// connection/transaction so writes (EF) and reads (Dapper) are atomic when needed.
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    /// <summary>Open a DbConnection that participates in the current EF transaction (if any).</summary>
    Task<DbConnection> OpenConnectionAsync(CancellationToken ct = default);

    /// <summary>Commit all pending EF changes. Returns affected row count.</summary>
    Task<int> SaveChangesAsync(CancellationToken ct = default);

    /// <summary>Begin a database transaction. Disposing the returned token commits/rolls back.</summary>
    Task<IAsyncDisposable> BeginTransactionAsync(CancellationToken ct = default);
}