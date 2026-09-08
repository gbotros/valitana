namespace Valitana.Contracts;

/// <summary>
/// Published when a stock price changes.
/// </summary>
public sealed record StockPriceUpdated(
    string Symbol,
    decimal Price,
    DateTimeOffset OccurredAtUtc);
