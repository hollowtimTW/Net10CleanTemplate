using Npgsql;
using NpgsqlTypes;

namespace YourApp.Infrastructure.Persistence;

/// <summary>
/// Bulk writer using PostgreSQL COPY ... FROM STDIN — the fastest way to insert
/// tens of thousands of rows. ~30x faster than EF Core SaveChanges for large datasets.
/// </summary>
public sealed class NpgsqlBulkWriter(NpgsqlConnectionFactory factory)
{
    /// <summary>
    /// Bulk-insert rows via COPY. The <paramref name="writeRow"/> callback must call
    /// writer.StartRow + writer.Write* for each column.
    /// </summary>
    public async Task BulkCopyAsync(
        string destinationTable,
        IReadOnlyList<string> columns,
        int rowCount,
        Func<NpgsqlBinaryImporter, Task> writeRow,
        CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationTable);
        if (columns.Count == 0)
            throw new ArgumentException("At least one column required.", nameof(columns));
        ArgumentNullException.ThrowIfNull(writeRow);
        if (rowCount <= 0)
            return;

        await using var conn = await factory.CreateOpenAsync(ct);

        var columnList = string.Join(", ", columns.Select(c => $"\"{c}\""));
        var sql = $"COPY {destinationTable} ({columnList}) FROM STDIN (FORMAT BINARY)";

        await using var importer = await conn.BeginBinaryImportAsync(sql, ct);
        for (var i = 0; i < rowCount; i++)
        {
            await importer.StartRowAsync(ct);
            await writeRow(importer);
        }
        await importer.CompleteAsync(ct);
    }
}

/// <summary>
/// Convenience helpers for the common case where each row maps to the same column shape.
/// </summary>
public static class NpgsqlBinaryImporterExtensions
{
    public static Task WriteNullableAsync<T>(
        this NpgsqlBinaryImporter importer,
        T? value,
        NpgsqlDbType dbType,
        CancellationToken ct = default)
        where T : struct
    {
        if (value.HasValue)
            return importer.WriteAsync(value.Value, dbType, ct);
        return importer.WriteNullAsync(ct);
    }

    public static Task WriteNullableStringAsync(
        this NpgsqlBinaryImporter importer,
        string? value,
        CancellationToken ct = default)
        => string.IsNullOrEmpty(value)
            ? importer.WriteNullAsync(ct)
            : importer.WriteAsync(value, NpgsqlDbType.Text, ct);
}