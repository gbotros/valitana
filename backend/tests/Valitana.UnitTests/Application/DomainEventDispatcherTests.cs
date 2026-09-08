using Valitana.Application.DomainEvents;
using Valitana.Domain.Aggregates;
using Valitana.Domain.Events;
using Valitana.Domain.ValueObjects;
using Valitana.UnitTests.Fakes;

namespace Valitana.UnitTests.Application;

public sealed class DomainEventDispatcherTests
{
    [Fact]
    public async Task DispatchAsync_publishes_one_notification_per_event()
    {
        var publisher = new CapturingPublisher();
        var dispatcher = new DomainEventDispatcher(publisher);
        var first = new StockPriceRecorded(
            Symbol.From("AAPL"),
            Price.From(1m),
            new DateTimeOffset(2026, 9, 8, 16, 0, 0, TimeSpan.Zero));
        var second = new StockPriceRecorded(
            Symbol.From("AAPL"),
            Price.From(2m),
            new DateTimeOffset(2026, 9, 8, 16, 0, 1, TimeSpan.Zero));

        await dispatcher.DispatchAsync([first, second], CancellationToken.None);

        Assert.Equal(2, publisher.Published.Count);
        Assert.Equal(first, Assert.IsType<DomainEventNotification<StockPriceRecorded>>(publisher.Published[0]).DomainEvent);
        Assert.Equal(second, Assert.IsType<DomainEventNotification<StockPriceRecorded>>(publisher.Published[1]).DomainEvent);
    }

    [Fact]
    public async Task DispatchAsync_does_not_clear_the_aggregate()
    {
        var publisher = new CapturingPublisher();
        var dispatcher = new DomainEventDispatcher(publisher);
        var stock = new Stock(Symbol.From("AAPL"));
        stock.RecordPrice(Price.From(10m), new DateTimeOffset(2026, 9, 8, 16, 0, 0, TimeSpan.Zero));
        var alreadyDequeued = stock.DequeueDomainEvents();

        stock.RecordPrice(Price.From(11m), new DateTimeOffset(2026, 9, 8, 16, 0, 1, TimeSpan.Zero));

        await dispatcher.DispatchAsync(alreadyDequeued, CancellationToken.None);

        Assert.Single(stock.DequeueDomainEvents());
    }
}
