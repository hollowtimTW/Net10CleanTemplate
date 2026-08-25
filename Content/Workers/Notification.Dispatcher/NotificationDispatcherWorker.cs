using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YourApp.Notification;

namespace YourApp.Notification.Dispatcher;

public sealed class NotificationDispatcherWorker(
    ChannelReader<NotificationMessage> reader,
    ILogger<NotificationDispatcherWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Notification.Dispatcher started");

        await foreach (var msg in reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                // TODO: replace with SMTP / SMS / Push adapters.
                logger.LogInformation(
                    "NOTIFY [{Channel}] to {Recipient}: {Subject}",
                    msg.Channel, msg.Recipient, msg.Subject);
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                logger.LogError(ex, "Notification dispatch failed for {Channel} {Recipient}",
                    msg.Channel, msg.Recipient);
            }
        }

        logger.LogInformation("Notification.Dispatcher stopped");
    }
}