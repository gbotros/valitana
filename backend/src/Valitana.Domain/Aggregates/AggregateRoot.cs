using Valitana.Domain.Events;

namespace Valitana.Domain.Aggregates;

/// <summary>
/// Holds domain events raised by the aggregate until they move to the outbox.
/// </summary>
public abstract class AggregateRoot : IAggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// Records an event from a state change. Call from the aggregate only.
    /// </summary>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Returns pending events and clears the list. The repository writes them to the outbox.
    /// </summary>
    public IReadOnlyList<IDomainEvent> DequeueDomainEvents()
    {
        var events = _domainEvents.ToArray();
        _domainEvents.Clear();
        return events;
    }
}
