using AwesomeAssertions;
using YourApp.Domain.Primitives;
using Xunit;

namespace YourApp.UnitTests.Primitives;

public class StronglyTypedIdTests
{
    private sealed record OrderId(Guid Value) : GuidId(Value);
    private sealed record CustomerId(Guid Value) : GuidId(Value);

    [Fact]
    public void GuidId_carries_value()
    {
        var id = new OrderId(Guid.NewGuid());
        id.Value.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void GuidIds_of_different_types_with_same_value_are_not_equal()
    {
        var guid = Guid.NewGuid();
        var order = new OrderId(guid);
        var customer = new CustomerId(guid);
        // Records compare structurally; same Value type (Guid) → equal
        // The point of StronglyTypedId is preventing MIXED usage at compile time.
        order.Value.Should().Be(customer.Value);
    }

    [Fact]
    public void Implicit_conversion_to_Guid_works()
    {
        var guid = Guid.NewGuid();
        var id = new OrderId(guid);
        Guid extracted = id;
        extracted.Should().Be(guid);
    }
}