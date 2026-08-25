namespace YourApp.Domain.Primitives;

/// <summary>
/// Base class for aggregate roots. Aggregates are consistency boundaries —
/// they own their invariants and emit domain events for state changes.
/// </summary>
public abstract class AggregateRoot<TId>
    where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public TId Id { get; protected set; } = default!;
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent @event) => _domainEvents.Add(@event);

    /// <summary>
    /// Called by infrastructure after the aggregate is successfully persisted.
    /// Clears the event queue so the next persistence cycle does not re-dispatch.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();

    /// <summary>
    /// Override to perform any post-creation initialization (e.g. emit CreatedEvent).
    /// Called by Create factory methods in derived classes.
    /// </summary>
    protected virtual void OnCreated() { }
}