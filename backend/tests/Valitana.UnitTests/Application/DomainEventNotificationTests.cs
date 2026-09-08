using Valitana.Application.DomainEvents;
using Valitana.Domain.Events;
using Valitana.Domain.ValueObjects;

namespace Valitana.UnitTests.Application;

public sealed class DomainEventNotificationTests
{
    [Fact]
    public void From_wraps_as_typed_StockPriceRecorded_notification()
    {
        var domainEvent = new StockPriceRecorded(
            Symbol.From("AAPL"),
            Price.From(1m),
            new DateTimeOffset(2026, 9, 8, 15, 0, 0, TimeSpan.Zero));

        var notification = DomainEventNotification.From(domainEvent);

        var typed = Assert.IsType<DomainEventNotification<StockPriceRecorded>>(notification);
        Assert.Same(domainEvent, typed.DomainEvent);
    }
}
