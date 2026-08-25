using AcmeApp.Api.Application.Catalog;
using AcmeApp.Api.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YourApp.Application.DependencyInjection;
using YourApp.Infrastructure.DependencyInjection;
using YourApp.Infrastructure.Persistence;

namespace AcmeApp.Api.Infrastructure;

public sealed class AcmeAppDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();

    public AcmeAppDbContext(DbContextOptions<AcmeAppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Product>(e =>
        {
            e.ToTable("products");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasConversion(p => p.Value, v => new ProductId(v));
            e.Property(x => x.Sku).HasMaxLength(64).IsRequired();
            e.Property(x => x.Name).HasMaxLength(256).IsRequired();
            e.OwnsOne(x => x.Price, m =>
            {
                m.Property(p => p.Amount).HasColumnName("price_amount").HasColumnType("numeric(18,4)");
                m.Property(p => p.Currency).HasColumnName("price_currency").HasMaxLength(3).IsRequired();
            });
            e.Property(x => x.StockQuantity).IsRequired();
            e.Property(x => x.IsActive).IsRequired();
        });
    }
}

public sealed class EfProductRepository(AcmeAppDbContext db) : IProductRepository
{
    public async ValueTask AddAsync(Product product, CancellationToken ct)
        => await db.Products.AddAsync(product, ct);

    public async ValueTask<Product?> GetAsync(ProductId id, CancellationToken ct)
        => await db.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
}

public static class AcmeAppModuleExtensions
{
    public static IServiceCollection AddAcmeAppModule(
        this IServiceCollection services,
        IConfiguration cfg)
    {
        var connStr = cfg.GetConnectionString("Database")
            ?? throw new InvalidOperationException("ConnectionStrings:Database is required.");
        services.AddYourAppInfrastructure<AcmeAppDbContext>(opt => { }, connStr);
        services.AddScoped<EfProductRepository>();
        services.AddScoped<IProductRepository>(sp =>
            sp.GetRequiredService<EfProductRepository>());
        return services;
    }
}

public static class AcmeAppApplicationExtensions
{
    public static IServiceCollection AddAcmeAppApplication(this IServiceCollection services)
        => services.AddYourAppApplication(typeof(CreateProductValidator).Assembly);
}