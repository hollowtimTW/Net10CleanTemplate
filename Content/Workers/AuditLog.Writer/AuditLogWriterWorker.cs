using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YourApp.AuditLog;

namespace YourApp.AuditLog.Writer;

public sealed class AuditLogWriterWorker(
    ChannelReader<AuditEvent> reader,
    ILogger<AuditLogWriterWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("AuditLog.Writer started");

        await foreach (var evt in reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                // TODO: replace with DB sink. For now, structured log.
                logger.LogInformation(
                    "AUDIT {Action} {ResourceType}#{ResourceId} user={UserId} ip={Ip}",
                    evt.Action, evt.ResourceType, evt.ResourceId, evt.UserId ?? "-", evt.IpAddress ?? "-");
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                logger.LogError(ex, "Audit sink failed for {Action} {ResourceType}#{ResourceId}",
                    evt.Action, evt.ResourceType, evt.ResourceId);
            }
        }

        logger.LogInformation("AuditLog.Writer stopped");
    }
}