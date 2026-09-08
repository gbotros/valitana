using Valitana.Domain.Events;
using Valitana.Domain.ValueObjects;
using Valitana.Infrastructure.Persistence;

namespace Valitana.UnitTests.Infrastructure;

public sealed class OutboxMessageMapperTests
{
    [Fact]
    public void Round_trips_StockPriceRecorded()
    {
        var occurredAt = new DateTimeOffset(2026, 9, 8, 17, 30, 0, TimeSpan.Zero);
        var original = new StockPriceRecorded(Symbol.From("aapl"), Price.From(150.2500m), occurredAt);

        var message = OutboxMessageMapper.ToOutboxMessage(original);
        var restored = OutboxMessageMapper.ToDomainEvent(message);

        var recorded = Assert.IsType<StockPriceRecorded>(restored);
        Assert.Equal("AAPL", recorded.Symbol.Value);
        Assert.Equal(150.2500m, recorded.Price.Value);
        Assert.Equal(occurredAt, recorded.OccurredAtUtc);
        Assert.Equal(nameof(StockPriceRecorded), message.Type);
        Assert.False(string.IsNullOrWhiteSpace(message.Payload));
        Assert.NotEqual(Guid.Empty, message.Id);
    }

    [Fact]
    public void ToOutboxMessage_rejects_unknown_domain_event_type()
    {
        var ex = Assert.Throws<InvalidOperationException>(
            () => OutboxMessageMapper.ToOutboxMessage(new UnsupportedDomainEvent()));

        Assert.Contains("Unsupported domain event", ex.Message);
    }

    [Fact]
    public void ToDomainEvent_rejects_unknown_outbox_type()
    {
        var message = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = "SomethingElse",
            Payload = "{}",
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        var ex = Assert.Throws<InvalidOperationException>(() => OutboxMessageMapper.ToDomainEvent(message));

        Assert.Contains("Unsupported outbox type", ex.Message);
    }

    [Fact]
    public void ToDomainEvent_rejects_empty_payload()
    {
        var message = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = nameof(StockPriceRecorded),
            Payload = "null",
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        var ex = Assert.Throws<InvalidOperationException>(() => OutboxMessageMapper.ToDomainEvent(message));

        Assert.Contains("empty", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    private sealed record UnsupportedDomainEvent : IDomainEvent;
}
