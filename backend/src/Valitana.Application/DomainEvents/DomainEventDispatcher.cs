using MediatR;
using Valitana.Application.Abstractions;
using Valitana.Domain.Events;

namespace Valitana.Application.DomainEvents;

/// <summary>
/// Publishes domain events through MediatR. Does not clear the aggregate.
/// </summary>
public sealed class DomainEventDispatcher(IPublisher publisher) : IDomainEventDispatcher
{
    public async Task DispatchAsync(
        IReadOnlyList<IDomainEvent> events,
        CancellationToken cancellationToken)
    {
        foreach (var domainEvent in events)
        {
            await publisher.Publish(DomainEventNotification.From(domainEvent), cancellationToken);
        }
    }
}
