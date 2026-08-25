using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Threading.Channels;
using YourApp.AuditLog;
using YourApp.AuditLog.Writer;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSerilog((sp, lc) =>
    lc.ReadFrom.Configuration(builder.Configuration)
      .Enrich.FromLogContext()
      .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] [AuditLogWriter] {Message:lj} {Properties:j}{NewLine}{Exception}"));

builder.Services.AddSingleton(_ => Channel.CreateBounded<AuditEvent>(new BoundedChannelOptions(10_000)
{
    FullMode = BoundedChannelFullMode.DropOldest,
    SingleReader = true,
    SingleWriter = false
}));
builder.Services.AddSingleton<ChannelReader<AuditEvent>>(sp =>
    sp.GetRequiredService<Channel<AuditEvent>>().Reader);
builder.Services.AddHostedService<AuditLogWriterWorker>();

var host = builder.Build();
await host.RunAsync();