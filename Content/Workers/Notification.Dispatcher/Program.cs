using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Threading.Channels;
using YourApp.Notification;
using YourApp.Notification.Dispatcher;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSerilog((sp, lc) =>
    lc.ReadFrom.Configuration(builder.Configuration)
      .Enrich.FromLogContext()
      .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] [NotificationDispatcher] {Message:lj} {Properties:j}{NewLine}{Exception}"));

builder.Services.AddSingleton(_ => Channel.CreateBounded<NotificationMessage>(new BoundedChannelOptions(10_000)
{
    FullMode = BoundedChannelFullMode.DropOldest,
    SingleReader = true,
    SingleWriter = false
}));
builder.Services.AddSingleton<ChannelReader<NotificationMessage>>(sp =>
    sp.GetRequiredService<Channel<NotificationMessage>>().Reader);
builder.Services.AddHostedService<NotificationDispatcherWorker>();

var host = builder.Build();
await host.RunAsync();