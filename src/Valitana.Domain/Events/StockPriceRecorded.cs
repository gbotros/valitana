using Valitana.Domain.ValueObjects;

namespace Valitana.Domain.Events;

/// <summary>
/// Raised by the Stock aggregate whenever a new price is recorded.
/// </summary>
public sealed record StockPriceRecorded(
    Symbol Symbol,
    Price Price,
    DateTimeOffset OccurredAtUtc) : IDomainEvent;
