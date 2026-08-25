namespace YourApp.Domain.Primitives;

/// <summary>
/// Marker for domain events. Domain events are facts that happened in the past tense
/// (OrderPlacedEvent, not PlaceOrderCommand). They are collected by aggregates and
/// dispatched by infrastructure (mediator / outbox / channel).
/// </summary>
public interface IDomainEvent
{
    DateTime OccurredAt { get; }
}