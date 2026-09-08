using Valitana.Domain.Time;

namespace Valitana.Infrastructure.Time;

/// <summary>
/// Wall clock. Swap in tests.
/// </summary>
public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
