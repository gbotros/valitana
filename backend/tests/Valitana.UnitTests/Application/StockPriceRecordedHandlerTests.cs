using Valitana.Application.DomainEvents;
using Valitana.Contracts;
using Valitana.Domain.Events;
using Valitana.Domain.ValueObjects;
using Valitana.UnitTests.Fakes;

namespace Valitana.UnitTests.Application;

public sealed class StockPriceRecordedHandlerTests
{
    [Fact]
    public async Task Handle_maps_domain_event_to_StockPriceUpdated()
    {
        var publisher = new CapturingIntegrationEventPublisher();
        var handler = new StockPriceRecordedHandler(publisher);
        var occurredAt = new DateTimeOffset(2026, 9, 8, 14, 0, 0, TimeSpan.Zero);
        var domainEvent = new StockPriceRecorded(Symbol.From("AAPL"), Price.From(150.25m), occurredAt);

        await handler.Handle(new DomainEventNotification<StockPriceRecorded>(domainEvent), CancellationToken.None);

        var published = Assert.IsType<StockPriceUpdated>(Assert.Single(publisher.Published));
        Assert.Equal("AAPL", published.Symbol);
        Assert.Equal(150.25m, published.Price);
        Assert.Equal(occurredAt, published.OccurredAtUtc);
    }
}
