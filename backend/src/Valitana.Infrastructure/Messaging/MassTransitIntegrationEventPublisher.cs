using MassTransit;
using Microsoft.Extensions.Logging;
using Valitana.Application.Abstractions;

namespace Valitana.Infrastructure.Messaging;

public sealed class MassTransitIntegrationEventPublisher(
    IPublishEndpoint publishEndpoint,
    ILogger<MassTransitIntegrationEventPublisher> logger) : IIntegrationEventPublisher
{
    public async Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken)
        where TEvent : class
    {
        await publishEndpoint.Publish(integrationEvent, cancellationToken);
        logger.LogInformation("Published {EventType}", typeof(TEvent).Name);
    }
}
