namespace Valitana.Application.Abstractions;

/// <summary>
/// Publishes integration events to the broker.
/// </summary>
public interface IIntegrationEventPublisher
{
    Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken)
        where TEvent : class;
}
