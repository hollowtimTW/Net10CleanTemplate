using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace YourApp.Shared.Hosting;

/// <summary>
/// One-line registration for the standard middleware stack:
/// Serilog → ProblemDetails → ExceptionHandler → Swagger (Dev only) → HealthCheck.
/// Wire up in Program.cs: services.AddYourAppHosting("MyApp");
/// then app.UseYourAppDefaults();
/// </summary>
public static class HostingExtensions
{
    public static IServiceCollection AddYourAppHosting(
        this IServiceCollection services,
        string subsystemName,
        Action<LoggerConfiguration>? configureLogger = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(subsystemName);

        services.AddProblemDetails();
        services.AddHttpContextAccessor();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = $"{subsystemName} API",
                Version = "v1"
            });
        });

        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy("alive"));

        // Serilog as the host logger
        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .Enrich.WithProperty("Subsystem", subsystemName)
            .WriteTo.Console(outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level:u3}] [{Subsystem}] {Message:lj} {Properties:j}{NewLine}{Exception}");
        configureLogger?.Invoke(loggerConfig);
        Log.Logger = loggerConfig.CreateLogger();

        services.AddSerilog((sp, lc) =>
        {
            lc.ReadFrom.Configuration(sp.GetRequiredService<IConfiguration>());
            lc.Enrich.FromLogContext();
            lc.Enrich.WithProperty("Subsystem", subsystemName);
            configureLogger?.Invoke(lc);
        });

        return services;
    }

    public static WebApplication UseYourAppDefaults(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseExceptionHandler();
        app.UseStatusCodePages();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseSerilogRequestLogging(opt =>
        {
            opt.MessageTemplate =
                "HTTP {RequestMethod} {RequestPath} → {StatusCode} in {Elapsed:0.0000} ms";
            opt.EnrichDiagnosticContext = (diag, http) =>
            {
                diag.Set("RequestId", http.TraceIdentifier);
                diag.Set("ClientIP", http.Connection.RemoteIpAddress?.ToString());
                if (http.User.Identity?.IsAuthenticated == true)
                    diag.Set("UserName", http.User.Identity.Name);
            };
        });

        app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = _ => false // liveness: process is up
        });
        app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready")
        });

        return app;
    }
}