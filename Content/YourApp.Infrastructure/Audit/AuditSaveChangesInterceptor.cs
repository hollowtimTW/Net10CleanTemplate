using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using YourApp.Application.Abstractions;

namespace YourApp.Infrastructure.Audit;

/// <summary>
/// EF Core SaveChanges interceptor that emits audit events for each added/modified/deleted entity.
/// Hooks into the same pipeline as AuditSaveChangesInterceptor in the original hospital project,
/// but generalized to work with any entity type.
/// </summary>
public sealed class AuditSaveChangesInterceptor(ICurrentUser currentUser, IAuditWriter auditWriter)
    : SaveChangesInterceptor
{
    private readonly ICurrentUser _currentUser = currentUser;
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken ct = default)
    {
        await EmitAuditEventsAsync(eventData.Context, ct);
        return await base.SavingChangesAsync(eventData, result, ct);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken ct = default)
    {
        await base.SavedChangesAsync(eventData, result, ct);
        return result;
    }

    private async Task EmitAuditEventsAsync(DbContext? context, CancellationToken ct)
    {
        if (context is null) return;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.State is EntityState.Detached or EntityState.Unchanged)
                continue;

            var resourceType = entry.Entity.GetType().Name;
            var resourceId = entry.Properties
                .FirstOrDefault(p => p.Metadata.IsPrimaryKey())
                ?.CurrentValue
                ?.ToString() ?? "<unknown>";

            var action = entry.State switch
            {
                EntityState.Added => "CREATE",
                EntityState.Modified => "UPDATE",
                EntityState.Deleted => "DELETE",
                _ => "UNKNOWN"
            };

            await auditWriter.RecordChangeAsync(
                action,
                resourceType,
                resourceId,
                before: entry.State == EntityState.Modified ? Snapshot(entry, original: true) : null,
                after: entry.State is EntityState.Added or EntityState.Modified ? Snapshot(entry, original: false) : null,
                ct: ct);
        }
    }

    private static Dictionary<string, object?> Snapshot(EntityEntry entry, bool original)
    {
        var snap = new Dictionary<string, object?>();
        foreach (var prop in entry.Properties)
        {
            if (prop.Metadata.IsPrimaryKey()) continue;
            snap[prop.Metadata.Name] = original ? prop.OriginalValue : prop.CurrentValue;
        }
        return snap;
    }
}