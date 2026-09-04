namespace Valitana.Application.Abstractions;

/// <summary>A single historical price tick (read model, not part of the aggregate).</summary>
public sealed record PriceTick(string Symbol, decimal Price, DateTimeOffset OccurredAtUtc);

/// <summary>
/// Port for the in-memory price history read model. History is never used for a
/// domain decision, so it lives outside the aggregate. Implementations keep at
/// most a configured number of ticks and discard the oldest tick on overflow.
/// </summary>
public interface IPriceHistoryStore
{
    void Append(PriceTick tick);

    IReadOnlyList<PriceTick> Snapshot();
}
