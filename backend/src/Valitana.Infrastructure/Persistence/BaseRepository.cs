using Microsoft.EntityFrameworkCore;
using Valitana.Application.Abstractions;
using Valitana.Domain.Aggregates;

namespace Valitana.Infrastructure.Persistence;

/// <summary>
/// Writes domain events to the outbox, saves, then publishes pending rows.
/// </summary>
public abstract class BaseRepository(
    ValitanaDbContext context,
    IDomainEventDispatcher dispatcher)
{
    protected ValitanaDbContext Context { get; } = context;

    protected async Task SaveAndDispatchAsync(AggregateRoot aggregate, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in aggregate.DequeueDomainEvents())
        {
            Context.OutboxMessages.Add(OutboxMessageMapper.ToOutboxMessage(domainEvent));
        }

        await Context.SaveChangesAsync(cancellationToken);

        var pending = await Context.OutboxMessages
            .Where(message => message.ProcessedAtUtc == null)
            .OrderBy(message => message.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        var events = pending.Select(OutboxMessageMapper.ToDomainEvent).ToList();
        await dispatcher.DispatchAsync(events, cancellationToken);

        var processedAtUtc = DateTimeOffset.UtcNow;
        foreach (var message in pending)
        {
            message.ProcessedAtUtc = processedAtUtc;
        }

        await Context.SaveChangesAsync(cancellationToken);
    }
}
