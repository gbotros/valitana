namespace Valitana.Domain.Time;

/// <summary>
/// Current UTC time. Domain and Application use this instead of DateTimeOffset.UtcNow.
/// </summary>
public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
