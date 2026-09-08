using Valitana.Application.Abstractions;
using Valitana.Domain.Aggregates;
using Valitana.Domain.ValueObjects;

namespace Valitana.UnitTests.Fakes;

internal sealed class FakeStockRepository : IStockRepository
{
    private readonly Dictionary<string, Stock> _items = new(StringComparer.Ordinal);

    public int UpsertCount { get; private set; }

    public IReadOnlyDictionary<string, Stock> Items => _items;

    public Stock? GetStock(Symbol symbol)
    {
        return _items.TryGetValue(symbol.Value, out var stock) ? stock : null;
    }

    public Task Upsert(Stock stock, CancellationToken cancellationToken)
    {
        _items[stock.Symbol.Value] = stock;
        UpsertCount++;
        return Task.CompletedTask;
    }
}
