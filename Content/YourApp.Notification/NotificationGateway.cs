using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;

namespace YourApp.Notification;

public sealed record NotificationMessage(
    string Channel,            // "email" | "sms" | "push"
    string Recipient,
    string Subject,
    string Body,
    IReadOnlyDictionary<string, string>? Metadata = null);

public interface INotificationGateway
{
    ValueTask SendAsync(NotificationMessage message, CancellationToken ct = default);
}

public sealed class NullNotificationGateway : INotificationGateway
{
    public ValueTask SendAsync(NotificationMessage message, CancellationToken ct = default) => ValueTask.CompletedTask;
}

public sealed class ChannelNotificationDispatcher(ChannelWriter<NotificationMessage> writer) : INotificationGateway
{
    public async ValueTask SendAsync(NotificationMessage message, CancellationToken ct = default)
        => await writer.WriteAsync(message, ct);
}

public static class NotificationExtensions
{
    public static IServiceCollection AddYourAppNotification(this IServiceCollection services)
    {
        services.AddSingleton(_ =>
        {
            var channel = Channel.CreateBounded<NotificationMessage>(new BoundedChannelOptions(10_000)
            {
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleReader = true,
                SingleWriter = false
            });
            return channel;
        });
        services.AddSingleton<ChannelReader<NotificationMessage>>(sp =>
            sp.GetRequiredService<Channel<NotificationMessage>>().Reader);
        services.AddSingleton<ChannelWriter<NotificationMessage>>(sp =>
            sp.GetRequiredService<Channel<NotificationMessage>>().Writer);
        services.AddSingleton<INotificationGateway, ChannelNotificationDispatcher>();
        return services;
    }
}