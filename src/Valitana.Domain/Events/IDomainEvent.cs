namespace Valitana.Domain.Events;

/// <summary>
/// Marker for domain events. The Domain project has no framework dependencies;
/// the Application layer adapts these to its own eventing infrastructure.
/// </summary>
public interface IDomainEvent
{
    DateTimeOffset OccurredAtUtc { get; }
}
