using AwesomeAssertions;
using YourApp.Domain.Primitives;
using Xunit;

namespace YourApp.UnitTests.Primitives;

public class AggregateRootTests
{
    private sealed record ItemId(Guid Value) : GuidId(Value);
    private sealed record ItemAddedEvent(ItemId Id) : IDomainEvent
    {
        public DateTime OccurredAt { get; } = DateTime.UtcNow;
    }

    private sealed class Item : AggregateRoot<ItemId>
    {
        public static Item Create() => new() { Id = new ItemId(Guid.NewGuid()) };
        public void Touch() => AddDomainEvent(new ItemAddedEvent(Id));
    }

    [Fact]
    public void New_aggregate_has_no_domain_events()
    {
        var item = Item.Create();
        item.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Adding_event_records_it()
    {
        var item = Item.Create();
        item.Touch();
        item.DomainEvents.Should().HaveCount(1);
        item.DomainEvents.First().Should().BeOfType<ItemAddedEvent>();
    }

    [Fact]
    public void ClearDomainEvents_removes_all_events()
    {
        var item = Item.Create();
        item.Touch();
        item.ClearDomainEvents();
        item.DomainEvents.Should().BeEmpty();
    }
}