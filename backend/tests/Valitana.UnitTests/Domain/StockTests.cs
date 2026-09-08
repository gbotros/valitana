using Valitana.Domain.Aggregates;
using Valitana.Domain.Events;
using Valitana.Domain.ValueObjects;

namespace Valitana.UnitTests.Domain;

public sealed class StockTests
{
    private static readonly DateTimeOffset OccurredAt = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void RecordPrice_sets_state_and_raises_StockPriceRecorded()
    {
        var stock = new Stock(Symbol.From("AAPL"));
        var price = Price.From(150.25m);

        stock.RecordPrice(price, OccurredAt);

        Assert.Equal(price, stock.CurrentPrice);
        Assert.Equal(OccurredAt, stock.LastUpdatedAtUtc);

        var recorded = Assert.Single(stock.DequeueDomainEvents());
        var domainEvent = Assert.IsType<StockPriceRecorded>(recorded);
        Assert.Equal(stock.Symbol, domainEvent.Symbol);
        Assert.Equal(price, domainEvent.Price);
        Assert.Equal(OccurredAt, domainEvent.OccurredAtUtc);
    }

    [Fact]
    public void RecordPrice_rejects_default_occurrence_time()
    {
        var stock = new Stock(Symbol.From("AAPL"));

        var ex = Assert.Throws<ArgumentException>(() => stock.RecordPrice(Price.From(1m), default));

        Assert.Equal("occurredAtUtc", ex.ParamName);
        Assert.Null(stock.CurrentPrice);
        Assert.Null(stock.LastUpdatedAtUtc);
        Assert.Empty(stock.DequeueDomainEvents());
    }

    [Fact]
    public void RecordPrice_replaces_current_price_and_queues_another_event()
    {
        var stock = new Stock(Symbol.From("AAPL"));
        var first = Price.From(100m);
        var second = Price.From(101.5m);
        var secondOccurredAt = OccurredAt.AddSeconds(1);

        stock.RecordPrice(first, OccurredAt);
        stock.RecordPrice(second, secondOccurredAt);

        Assert.Equal(second, stock.CurrentPrice);
        Assert.Equal(secondOccurredAt, stock.LastUpdatedAtUtc);

        var events = stock.DequeueDomainEvents();
        Assert.Equal(2, events.Count);
        Assert.Equal(first, Assert.IsType<StockPriceRecorded>(events[0]).Price);
        Assert.Equal(second, Assert.IsType<StockPriceRecorded>(events[1]).Price);
    }

    [Fact]
    public void DequeueDomainEvents_returns_pending_events_then_clears()
    {
        var stock = new Stock(Symbol.From("MSFT"));
        stock.RecordPrice(Price.From(400m), OccurredAt);

        var first = stock.DequeueDomainEvents();
        var second = stock.DequeueDomainEvents();

        Assert.Single(first);
        Assert.Empty(second);
    }
}
