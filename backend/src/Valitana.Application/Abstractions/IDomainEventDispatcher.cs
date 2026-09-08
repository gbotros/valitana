using Valitana.Domain.Events;

namespace Valitana.Application.Abstractions;

/// <summary>
/// Publishes domain events through MediatR.
/// </summary>
public interface IDomainEventDispatcher
{
    Task DispatchAsync(IReadOnlyList<IDomainEvent> events, CancellationToken cancellationToken);
}
