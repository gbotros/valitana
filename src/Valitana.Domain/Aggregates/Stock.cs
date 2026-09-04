using Valitana.Domain.Events;
using Valitana.Domain.ValueObjects;

namespace Valitana.Domain.Aggregates;

/// <summary>
/// Aggregate root identified by its Symbol. Holds only the state it needs to
/// make decisions (the current price); historical ticks are a read model owned
/// by the Application layer, not part of this aggregate.
/// </summary>
public sealed class Stock
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public Symbol Symbol { get; }
    public Price? CurrentPrice { get; private set; }
    public DateTimeOffset? LastUpdatedAtUtc { get; private set; }

    public Stock(Symbol symbol) => Symbol = symbol;

    public void RecordPrice(Price price, DateTimeOffset occurredAtUtc)
    {
        CurrentPrice = price;
        LastUpdatedAtUtc = occurredAtUtc;
        _domainEvents.Add(new StockPriceRecorded(Symbol, price, occurredAtUtc));
    }

    /// <summary>Returns the pending domain events and clears the internal list.</summary>
    public IReadOnlyList<IDomainEvent> DequeueDomainEvents()
    {
        var events = _domainEvents.ToArray();
        _domainEvents.Clear();
        return events;
    }
}
