using Valitana.Domain.Aggregates;
using Valitana.Domain.ValueObjects;

namespace Valitana.Application.Abstractions;

/// <summary>Port for the Stock aggregate store. Implemented in Infrastructure (in-memory).</summary>
public interface IStockRepository
{
    Stock GetOrAdd(Symbol symbol);

    void Save(Stock stock);
}
