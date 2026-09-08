namespace Valitana.Infrastructure.Persistence;

/// <summary>
/// Durable copy of a domain event. Written in the same SaveChanges as the stock.
/// </summary>
public sealed class OutboxMessage
{
    public Guid Id { get; set; }
    public required string Type { get; set; }
    public required string Payload { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset? ProcessedAtUtc { get; set; }
}
