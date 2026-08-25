using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YourApp.AuditLog;
using YourApp.FileStorage;
using YourApp.Identity;
using YourApp.Notification;
using YourApp.Shared.Hosting;

var builder = WebApplication.CreateBuilder(args);

// --- One-line template wiring ---
builder.Services.AddYourAppHosting("YourApp.WebApi");
// services.AddYourAppApplication(typeof(SomeHandler).Assembly);  // <- add when you have handlers
// services.AddYourAppInfrastructure<YourDbContext>(opt => { }, builder.Configuration.GetConnectionString("Database")!); // <- add when you have a DbContext
builder.Services.AddYourAppIdentity(opt =>
{
    opt.EnableJwt = true;
    opt.EnableWindows = false;
});
builder.Services.AddYourAppAuditLog();
builder.Services.AddYourAppNotification();
builder.Services.AddLocalFileStorage(@"C:\YourApp\uploads");

var app = builder.Build();
app.UseYourAppDefaults();

// --- Sample endpoints (replace with your own) ---
app.MapGet("/", () => Results.Ok(new { name = "YourApp.WebApi", status = "running" }));
app.MapGet("/api/health", () => Results.Ok(new { status = "healthy", utc = DateTime.UtcNow }));

await app.RunAsync();

public partial class Program { }