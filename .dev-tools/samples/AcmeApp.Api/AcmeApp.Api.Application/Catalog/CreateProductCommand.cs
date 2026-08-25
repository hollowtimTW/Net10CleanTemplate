using AcmeApp.Api.Domain.Catalog;
using FluentValidation;
using MediatR;
using YourApp.Application.Abstractions;
using YourApp.Domain.Primitives;

namespace AcmeApp.Api.Application.Catalog;

public sealed record CreateProductCommand(string Sku, string Name, decimal Price, string Currency, int Stock)
    : ICommand<Result<ProductId>>;

public sealed class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Sku).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Currency).NotEmpty().Length(3);
        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
    }
}

public sealed class CreateProductHandler(IProductRepository repo, IUnitOfWork uow)
    : IRequestHandler<CreateProductCommand, Result<ProductId>>
{
    public async Task<Result<ProductId>> Handle(CreateProductCommand cmd, CancellationToken ct)
    {
        var productResult = Product.Create(
            cmd.Sku, cmd.Name,
            new Money(cmd.Price, cmd.Currency),
            cmd.Stock);
        if (productResult.IsFailed)
            return Result<ProductId>.Failure(productResult.Error);

        var product = productResult.Value;
        await repo.AddAsync(product, ct);
        await uow.SaveChangesAsync(ct);
        return Result<ProductId>.Success(product.Id);
    }
}

public interface IProductRepository
{
    ValueTask AddAsync(Product product, CancellationToken ct);
    ValueTask<Product?> GetAsync(ProductId id, CancellationToken ct);
}