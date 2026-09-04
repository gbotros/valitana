namespace Valitana.Application.Abstractions;

/// <summary>Port for publishing integration events to the message broker.</summary>
public interface IIntegrationEventPublisher
{
    Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken)
        where TEvent : class;
}
