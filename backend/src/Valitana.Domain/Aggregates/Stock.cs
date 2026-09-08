using Valitana.Domain.Events;
using Valitana.Domain.ValueObjects;

namespace Valitana.Domain.Aggregates;

public sealed class Stock : AggregateRoot
{
    public Symbol Symbol { get; private set; }
    public Price? CurrentPrice { get; private set; }
    public DateTimeOffset? LastUpdatedAtUtc { get; private set; }

    private Stock() => Symbol = null!;

    public Stock(Symbol symbol) => Symbol = symbol;

    public void RecordPrice(Price price, DateTimeOffset occurredAtUtc)
    {
        if (occurredAtUtc == default)
        {
            throw new ArgumentException("Occurrence time is required.", nameof(occurredAtUtc));
        }

        CurrentPrice = price;
        LastUpdatedAtUtc = occurredAtUtc;
        AddDomainEvent(new StockPriceRecorded(Symbol, price, occurredAtUtc));
    }
}
