using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using YourApp.Application.Abstractions;

namespace YourApp.AuditLog;

/// <summary>
/// Channel-based audit writer. Producers push to a bounded channel; the
/// AuditLog.Writer worker drains and persists. Decouples request latency
/// from audit write latency.
/// </summary>
public sealed class ChannelAuditWriter(ChannelWriter<AuditEvent> writer) : IAuditWriter
{
    public async ValueTask RecordReadAsync(string resourceType, string resourceId, CancellationToken ct = default)
    {
        var evt = new AuditEvent(
            Action: "READ",
            ResourceType: resourceType,
            ResourceId: resourceId,
            UserId: null, // populated by HostedChannelAuditWriter in the Web project
            IpAddress: null,
            CorrelationId: null,
            OccurredAtUtc: DateTime.UtcNow,
            PayloadJson: null);
        await writer.WriteAsync(evt, ct);
    }

    public async ValueTask RecordChangeAsync(
        string action,
        string resourceType,
        string resourceId,
        object? before = null,
        object? after = null,
        CancellationToken ct = default)
    {
        var payload = before is null && after is null
            ? null
            : System.Text.Json.JsonSerializer.Serialize(new { before, after });

        var evt = new AuditEvent(
            Action: action,
            ResourceType: resourceType,
            ResourceId: resourceId,
            UserId: null,
            IpAddress: null,
            CorrelationId: null,
            OccurredAtUtc: DateTime.UtcNow,
            PayloadJson: payload);
        await writer.WriteAsync(evt, ct);
    }
}

public static class AuditLogExtensions
{
    /// <summary>
    /// Registers the bounded channel + ChannelAuditWriter. Call once per process.
    /// </summary>
    public static IServiceCollection AddYourAppAuditLog(this IServiceCollection services, int capacity = 10_000)
    {
        services.AddSingleton(_ =>
        {
            var channel = Channel.CreateBounded<AuditEvent>(new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleReader = true,
                SingleWriter = false
            });
            return channel;
        });
        services.AddSingleton<ChannelReader<AuditEvent>>(sp =>
            sp.GetRequiredService<Channel<AuditEvent>>().Reader);
        services.AddSingleton<ChannelWriter<AuditEvent>>(sp =>
            sp.GetRequiredService<Channel<AuditEvent>>().Writer);
        services.AddScoped<IAuditWriter, ChannelAuditWriter>();
        return services;
    }
}