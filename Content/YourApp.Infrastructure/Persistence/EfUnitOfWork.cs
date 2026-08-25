using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;
using YourApp.Application.Abstractions;

namespace YourApp.Infrastructure.Persistence;

/// <summary>
/// EF Core implementation of IUnitOfWork. Lets Dapper share the same DbConnection
/// so a write (EF) + read (Dapper) sequence is atomic within a single transaction.
/// </summary>
public sealed class EfUnitOfWork<TContext>(TContext db) : IUnitOfWork
    where TContext : DbContext
{
    private IDbContextTransaction? _activeTransaction;

    public async Task<DbConnection> OpenConnectionAsync(CancellationToken ct = default)
    {
        var conn = db.Database.GetDbConnection();
        if (conn.State != System.Data.ConnectionState.Open)
            await conn.OpenAsync(ct);
        return conn;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);

    public async Task<IAsyncDisposable> BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_activeTransaction is not null)
            return new NoOpDisposable(); // already in a tx; caller nests

        _activeTransaction = await db.Database.BeginTransactionAsync(ct);
        return new TxWrapper(this);
    }

    internal async Task CommitActiveAsync(CancellationToken ct = default)
    {
        if (_activeTransaction is null) return;
        await _activeTransaction.CommitAsync(ct);
        await _activeTransaction.DisposeAsync();
        _activeTransaction = null;
    }

    internal async Task RollbackActiveAsync()
    {
        if (_activeTransaction is null) return;
        await _activeTransaction.RollbackAsync();
        await _activeTransaction.DisposeAsync();
        _activeTransaction = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (_activeTransaction is not null)
        {
            await _activeTransaction.DisposeAsync();
            _activeTransaction = null;
        }
        await db.DisposeAsync();
    }

    private sealed class TxWrapper(EfUnitOfWork<TContext> uow) : IAsyncDisposable
    {
        public async ValueTask DisposeAsync()
        {
            try { await uow.CommitActiveAsync(); }
            catch { await uow.RollbackActiveAsync(); throw; }
        }
    }

    private sealed class NoOpDisposable : IAsyncDisposable
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}