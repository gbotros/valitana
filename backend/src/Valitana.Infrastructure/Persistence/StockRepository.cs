using Microsoft.EntityFrameworkCore;
using Valitana.Application.Abstractions;
using Valitana.Domain.Aggregates;
using Valitana.Domain.ValueObjects;

namespace Valitana.Infrastructure.Persistence;

/// <summary>
/// Loads and upserts stocks. Event dispatch lives on the base class.
/// </summary>
public sealed class StockRepository(
    ValitanaDbContext context,
    IDomainEventDispatcher dispatcher)
    : BaseRepository(context, dispatcher), IStockRepository
{
    public Stock? GetStock(Symbol symbol)
    {
        return Context.Stocks.Find(symbol);
    }

    public async Task Upsert(Stock stock, CancellationToken cancellationToken)
    {
        if (Context.Entry(stock).State == EntityState.Detached)
        {
            Context.Stocks.Add(stock);
        }

        await SaveAndDispatchAsync(stock, cancellationToken);
    }
}
