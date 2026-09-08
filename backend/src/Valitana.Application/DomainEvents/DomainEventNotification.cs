using MediatR;
using Valitana.Domain.Events;

namespace Valitana.Application.DomainEvents;

/// <summary>
/// Wraps a domain event so MediatR can handle it. Domain does not reference MediatR.
/// </summary>
public sealed record DomainEventNotification<TEvent>(TEvent DomainEvent) : INotification
    where TEvent : IDomainEvent;

/// <summary>
/// Builds the typed wrapper.
/// Dispatch only has IDomainEvent; handlers need the concrete type.
/// </summary>
public static class DomainEventNotification
{
    /// <summary>
    /// Uses the event's real type so the matching handler runs.
    /// </summary>
    public static INotification From(IDomainEvent domainEvent)
    {
        var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
        return (INotification)Activator.CreateInstance(notificationType, domainEvent)!;
    }
}
