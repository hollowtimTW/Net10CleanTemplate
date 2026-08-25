using AcmeApp.Api.Application.Catalog;
using AcmeApp.Api.Domain.Catalog;
using AcmeApp.Api.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YourApp.Identity;
using YourApp.AuditLog;
using YourApp.Notification;
using YourApp.FileStorage;
using YourApp.Shared.Hosting;

var builder = WebApplication.CreateBuilder(args);

// --- One-line template wiring ---
builder.Services.AddYourAppHosting("AcmeApp.Api");
builder.Services.AddAcmeAppApplication();      // MediatR + FluentValidation
builder.Services.AddAcmeAppModule(builder.Configuration); // EF + DbContext + Repos
builder.Services.AddYourAppIdentity(opt =>
{
    opt.EnableJwt = true;
    opt.EnableWindows = false;
});
builder.Services.AddYourAppAuditLog();
builder.Services.AddYourAppNotification();
builder.Services.AddLocalFileStorage(@"C:\AcmeApp\uploads");

var app = builder.Build();
app.UseYourAppDefaults();

// --- Sample endpoints ---
app.MapGet("/", () => Results.Ok(new { name = "AcmeApp.Api", status = "running" }));

app.MapPost("/api/products", async (
    CreateProductCommand cmd,
    ISender sender,
    CancellationToken ct) =>
{
    var result = await sender.Send(cmd, ct);
    return result.IsSuccess
        ? Results.Created($"/api/products/{result.Value.Value}", new { id = result.Value.Value })
        : Results.Problem(detail: result.Error.Message, statusCode: 400, title: result.Error.Code);
});

app.MapGet("/api/products/{id:guid}", async (
    Guid id,
    AcmeApp.Api.Application.Catalog.IProductRepository repo,
    CancellationToken ct) =>
{
    var product = await repo.GetAsync(new ProductId(id), ct);
    return product is null
        ? Results.NotFound()
        : Results.Ok(new { product.Id.Value, product.Sku, product.Name, Price = product.Price.ToString(), product.StockQuantity });
});

app.MapGet("/api/health", () => Results.Ok(new { status = "healthy", utc = DateTime.UtcNow }));

await app.RunAsync();

// Required for WebApplicationFactory<Program> in integration tests
public partial class Program { }