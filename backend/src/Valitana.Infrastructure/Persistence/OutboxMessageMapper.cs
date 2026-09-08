using System.Text.Json;
using Valitana.Domain.Events;
using Valitana.Domain.ValueObjects;

namespace Valitana.Infrastructure.Persistence;

/// <summary>
/// Serializes domain events to outbox rows and back.
/// </summary>
public static class OutboxMessageMapper
{
    public static OutboxMessage ToOutboxMessage(IDomainEvent domainEvent)
    {
        if (domainEvent is not StockPriceRecorded recorded)
        {
            throw new InvalidOperationException($"Unsupported domain event {domainEvent.GetType().Name}.");
        }

        var payload = new StockPriceRecordedPayload(
            recorded.Symbol.Value,
            recorded.Price.Value,
            recorded.OccurredAtUtc);

        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = nameof(StockPriceRecorded),
            Payload = JsonSerializer.Serialize(payload),
            CreatedAtUtc = DateTimeOffset.UtcNow
        };
    }

    public static IDomainEvent ToDomainEvent(OutboxMessage message)
    {
        if (message.Type != nameof(StockPriceRecorded))
        {
            throw new InvalidOperationException($"Unsupported outbox type {message.Type}.");
        }

        var payload = JsonSerializer.Deserialize<StockPriceRecordedPayload>(message.Payload)
            ?? throw new InvalidOperationException("Outbox payload is empty.");

        return new StockPriceRecorded(
            Symbol.From(payload.Symbol),
            Price.From(payload.Price),
            payload.OccurredAtUtc);
    }

    private sealed record StockPriceRecordedPayload(string Symbol, decimal Price, DateTimeOffset OccurredAtUtc);
}
