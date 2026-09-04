using Microsoft.Extensions.Options;
using Valitana.Application.Abstractions;

namespace Valitana.Infrastructure.Persistence;

/// <summary>
/// Thread-safe ring of price ticks. Keeps at most PriceHistoryOptions.MaxCount
/// entries and dequeues the oldest tick on overflow.
/// </summary>
public sealed class InMemoryPriceHistoryStore(IOptions<PriceHistoryOptions> options) : IPriceHistoryStore
{
    private readonly Queue<PriceTick> _ticks = new();
    private readonly Lock _lock = new();
    private readonly int _maxCount = options.Value.MaxCount;

    public void Append(PriceTick tick)
    {
        lock (_lock)
        {
            _ticks.Enqueue(tick);
            while (_ticks.Count > _maxCount)
            {
                _ticks.Dequeue();
            }
        }
    }

    public IReadOnlyList<PriceTick> Snapshot()
    {
        lock (_lock)
        {
            return [.. _ticks];
        }
    }
}
