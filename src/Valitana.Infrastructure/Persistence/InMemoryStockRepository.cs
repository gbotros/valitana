using System.Collections.Concurrent;
using Valitana.Application.Abstractions;
using Valitana.Domain.Aggregates;
using Valitana.Domain.ValueObjects;

namespace Valitana.Infrastructure.Persistence;

public sealed class InMemoryStockRepository : IStockRepository
{
    private readonly ConcurrentDictionary<string, Stock> _stocks = new();

    public Stock GetOrAdd(Symbol symbol) =>
        _stocks.GetOrAdd(symbol.Value, _ => new Stock(symbol));

    public void Save(Stock stock) => _stocks[stock.Symbol.Value] = stock;
}
