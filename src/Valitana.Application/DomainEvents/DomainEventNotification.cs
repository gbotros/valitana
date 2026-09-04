using MediatR;
using Valitana.Domain.Events;

namespace Valitana.Application.DomainEvents;

/// <summary>
/// Adapts a framework-free IDomainEvent to a MediatR INotification so the
/// Domain project never references MediatR.
/// </summary>
public sealed record DomainEventNotification<TEvent>(TEvent DomainEvent) : INotification
    where TEvent : IDomainEvent;

public static class DomainEventNotification
{
    /// <summary>Wraps a domain event in DomainEventNotification&lt;TEvent&gt; using its runtime type.</summary>
    public static INotification From(IDomainEvent domainEvent)
    {
        var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
        return (INotification)Activator.CreateInstance(notificationType, domainEvent)!;
    }
}
