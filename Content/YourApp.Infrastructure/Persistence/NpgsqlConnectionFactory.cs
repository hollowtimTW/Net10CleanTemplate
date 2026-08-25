using Npgsql;

namespace YourApp.Infrastructure.Persistence;

/// <summary>
/// Singleton wrapper around NpgsqlDataSource (the modern Npgsql connection pool).
/// Created once at startup and shared across the whole app.
/// </summary>
public sealed class NpgsqlConnectionFactory(NpgsqlDataSource dataSource)
{
    public NpgsqlDataSource DataSource => dataSource;

    public async Task<NpgsqlConnection> CreateOpenAsync(CancellationToken ct = default)
        => await dataSource.OpenConnectionAsync(ct);
}