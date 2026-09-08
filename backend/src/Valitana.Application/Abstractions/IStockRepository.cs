using Valitana.Domain.Aggregates;
using Valitana.Domain.ValueObjects;

namespace Valitana.Application.Abstractions;

/// <summary>
/// Load and save a stock. Upsert writes the stock and outbox, then publishes.
/// </summary>
public interface IStockRepository
{
    Stock? GetStock(Symbol symbol);

    Task Upsert(Stock stock, CancellationToken cancellationToken);
}
