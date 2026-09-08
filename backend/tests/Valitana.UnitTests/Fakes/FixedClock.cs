using Valitana.Domain.Time;

namespace Valitana.UnitTests.Fakes;

internal sealed class FixedClock(DateTimeOffset utcNow) : IClock
{
    public DateTimeOffset UtcNow { get; } = utcNow;
}
