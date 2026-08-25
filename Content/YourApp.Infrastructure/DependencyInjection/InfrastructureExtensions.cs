using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using YourApp.Application.Abstractions;
using YourApp.Infrastructure.Audit;
using YourApp.Infrastructure.Persistence;

namespace YourApp.Infrastructure.DependencyInjection;

public static class InfrastructureExtensions
{
    /// <summary>
    /// One-line registration: Postgres connection pool, EF Core options, audit interceptor,
    /// IUnitOfWork, IAuditWriter, IDateTime.
    /// </summary>
    /// <param name="services">The DI container.</param>
    /// <param name="configureDb">Action to call UseNpgsql / UseSqlServer on the options builder.</param>
    /// <param name="connectionString">Postgres connection string used for the NpgsqlDataSource pool.</param>
    public static IServiceCollection AddYourAppInfrastructure<TContext>(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> configureDb,
        string connectionString)
        where TContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureDb);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        // 1. Postgres data source (singleton pool)
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.EnableDynamicJson();
        services.AddSingleton(dataSourceBuilder.Build());

        services.AddSingleton<NpgsqlConnectionFactory>();

        // 2. EF Core + interceptor
        services.AddSingleton<AuditSaveChangesInterceptor>();
        services.AddDbContext<TContext>((sp, opt) =>
        {
            opt.UseNpgsql(connectionString);
            opt.AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>());
            configureDb(opt);
        });

        // 3. UoW + audit + clock
        services.AddScoped<IUnitOfWork>(sp => new EfUnitOfWork<TContext>(sp.GetRequiredService<TContext>()));
        services.AddScoped<IAuditWriter, StructuredLogAuditWriter>();
        services.AddSingleton<IDateTime, SystemDateTime>();

        return services;
    }
}