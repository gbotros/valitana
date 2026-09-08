using MediatR;
using Valitana.Application.Abstractions;
using Valitana.Contracts;
using Valitana.Domain.Events;

namespace Valitana.Application.DomainEvents;

public sealed class StockPriceRecordedHandler(IIntegrationEventPublisher integrationEventPublisher)
    : INotificationHandler<DomainEventNotification<StockPriceRecorded>>
{
    public Task Handle(
        DomainEventNotification<StockPriceRecorded> notification,
        CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var integrationEvent = new StockPriceUpdated(
            domainEvent.Symbol.Value,
            domainEvent.Price.Value,
            domainEvent.OccurredAtUtc);

        return integrationEventPublisher.PublishAsync(integrationEvent, cancellationToken);
    }
}
