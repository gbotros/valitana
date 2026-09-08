using Microsoft.EntityFrameworkCore;
using Valitana.Domain.Aggregates;

namespace Valitana.Infrastructure.Persistence;

/// <summary>
/// EF context. In-memory database shared across requests.
/// </summary>
public sealed class ValitanaDbContext(DbContextOptions<ValitanaDbContext> options) : DbContext(options)
{
    public DbSet<Stock> Stocks => Set<Stock>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ValitanaDbContext).Assembly);
    }
}
