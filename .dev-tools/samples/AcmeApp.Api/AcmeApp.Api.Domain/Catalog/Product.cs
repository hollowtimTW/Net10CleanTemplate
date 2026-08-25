using YourApp.Domain.Primitives;

namespace AcmeApp.Api.Domain.Catalog;

/// <summary>
/// A simple product in the catalog. Sample aggregate to prove the template works.
/// </summary>
public sealed class Product : AggregateRoot<ProductId>
{
    public string Sku { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public Money Price { get; private set; } = default!;
    public int StockQuantity { get; private set; }
    public bool IsActive { get; private set; }

    private Product() { } // EF

    public static Result<Product> Create(string sku, string name, Money price, int stock)
    {
        var id = new ProductId(Guid.NewGuid());
        var p = new Product
        {
            Id = id,
            Sku = sku,
            Name = name,
            Price = price,
            StockQuantity = stock,
            IsActive = true
        };
        p.AddDomainEvent(new ProductCreatedEvent(id, sku, name));
        return Result<Product>.Success(p);
    }

    public Result<Unit> Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            return Result.Failure(DomainError.Validation("Name cannot be empty."));
        Name = newName;
        AddDomainEvent(new ProductRenamedEvent(Id, newName));
        return Result.Success();
    }

    public Result<Unit> AdjustStock(int delta)
    {
        var next = StockQuantity + delta;
        if (next < 0)
            return Result.Failure(DomainError.Conflict("Stock cannot go negative."));
        StockQuantity = next;
        AddDomainEvent(new ProductStockAdjustedEvent(Id, delta, next));
        return Result.Success();
    }

    public Result<Unit> Deactivate()
    {
        if (!IsActive) return Result.Success();
        IsActive = false;
        AddDomainEvent(new ProductDeactivatedEvent(Id));
        return Result.Success();
    }
}

public sealed record ProductId(Guid Value) : GuidId(Value);

public sealed record ProductCreatedEvent(ProductId ProductId, string Sku, string Name) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public sealed record ProductRenamedEvent(ProductId ProductId, string NewName) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public sealed record ProductStockAdjustedEvent(ProductId ProductId, int Delta, int NewStock) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public sealed record ProductDeactivatedEvent(ProductId ProductId) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}