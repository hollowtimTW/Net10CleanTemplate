namespace YourApp.Application.Abstractions;

/// <summary>
/// Audit writer abstraction. Concrete implementation lives in Infrastructure.
/// Records who did what, when, from where, with what payload — for regulated
/// environments (HIPAA-equivalent / SOX / 政府法規).
/// </summary>
public interface IAuditWriter
{
    /// <summary>Record a data read (PHI / PII access, etc.).</summary>
    ValueTask RecordReadAsync(string resourceType, string resourceId, CancellationToken ct = default);

    /// <summary>Record a data change (create/update/delete).</summary>
    ValueTask RecordChangeAsync(
        string action,           // CREATE / UPDATE / DELETE
        string resourceType,
        string resourceId,
        object? before = null,
        object? after = null,
        CancellationToken ct = default);
}