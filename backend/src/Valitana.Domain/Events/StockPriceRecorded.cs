using Valitana.Domain.ValueObjects;

namespace Valitana.Domain.Events;

public sealed record StockPriceRecorded(
    Symbol Symbol,
    Price Price,
    DateTimeOffset OccurredAtUtc) : IDomainEvent;
