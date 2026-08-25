using YourApp.Domain.Primitives;

namespace AcmeApp.Api.Domain.Catalog;

/// <summary>
/// Money value object. Currency-aware, scale-aware. Sample value object.
/// </summary>
public sealed class Money : YourApp.Domain.Primitives.ValueObject, IEquatable<Money>
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency ?? throw new ArgumentNullException(nameof(currency));
    }

    public static Money Twd(decimal amount) => new(amount, "TWD");
    public static Money Usd(decimal amount) => new(amount, "USD");

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public bool Equals(Money? other) => other is not null && base.Equals(other);
    public override bool Equals(object? obj) => obj is Money m && Equals(m);
    public override int GetHashCode() => base.GetHashCode();

    public override string ToString() => $"{Amount:0.##} {Currency}";
}