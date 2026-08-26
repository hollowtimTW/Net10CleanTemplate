using AwesomeAssertions;
using YourApp.Domain.Primitives;
using Xunit;

namespace YourApp.UnitTests.Primitives;

public class ValueObjectTests
{
    private sealed class Money2 : ValueObject, IEquatable<Money2>
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money2(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }

        public bool Equals(Money2? other) => other is not null && base.Equals(other);
        public override bool Equals(object? obj) => obj is Money2 m && Equals(m);
        public override int GetHashCode() => base.GetHashCode();
    }

    [Fact]
    public void Two_value_objects_with_same_components_are_equal()
    {
        var a = new Money2(100m, "TWD");
        var b = new Money2(100m, "TWD");
        a.Equals(b).Should().BeTrue();
        (a == b).Should().BeTrue();
        (a != b).Should().BeFalse();
    }

    [Fact]
    public void Two_value_objects_with_different_components_are_not_equal()
    {
        var a = new Money2(100m, "TWD");
        var b = new Money2(200m, "TWD");
        a.Equals(b).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_is_consistent_with_Equals()
    {
        var a = new Money2(100m, "TWD");
        var b = new Money2(100m, "TWD");
        a.GetHashCode().Should().Be(b.GetHashCode());
    }
}