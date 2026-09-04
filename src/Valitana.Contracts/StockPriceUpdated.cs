namespace Valitana.Contracts;

/// <summary>
/// Integration event published to RabbitMQ when a stock price changes.
/// Shared by the API (publisher) and the SignalR service (consumer).
/// </summary>
public sealed record StockPriceUpdated(
    string Symbol,
    decimal Price,
    DateTimeOffset OccurredAtUtc);
