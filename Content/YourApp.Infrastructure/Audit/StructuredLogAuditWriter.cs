using Microsoft.Extensions.Logging;
using YourApp.Application.Abstractions;

namespace YourApp.Infrastructure.Audit;

/// <summary>
/// Default IAuditWriter that emits structured Serilog events.
/// Replace with a DB-backed implementation in regulated environments
/// (append-only audit table, 7-year retention, etc.).
/// </summary>
public sealed class StructuredLogAuditWriter(ILogger<StructuredLogAuditWriter> logger) : IAuditWriter
{
    public ValueTask RecordReadAsync(string resourceType, string resourceId, CancellationToken ct = default)
    {
        using (logger.BeginScope(new Dictionary<string, object?>
        {
            ["audit.action"] = "READ",
            ["audit.resourceType"] = resourceType,
            ["audit.resourceId"] = resourceId
        }))
        {
            logger.LogInformation("AUDIT READ {ResourceType} #{ResourceId}", resourceType, resourceId);
        }
        return ValueTask.CompletedTask;
    }

    public ValueTask RecordChangeAsync(
        string action,
        string resourceType,
        string resourceId,
        object? before = null,
        object? after = null,
        CancellationToken ct = default)
    {
        using (logger.BeginScope(new Dictionary<string, object?>
        {
            ["audit.action"] = action,
            ["audit.resourceType"] = resourceType,
            ["audit.resourceId"] = resourceId,
            ["audit.before"] = before,
            ["audit.after"] = after
        }))
        {
            logger.LogInformation("AUDIT {Action} {ResourceType} #{ResourceId}", action, resourceType, resourceId);
        }
        return ValueTask.CompletedTask;
    }
}