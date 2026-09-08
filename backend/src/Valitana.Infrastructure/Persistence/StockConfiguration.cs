using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Valitana.Domain.Aggregates;
using Valitana.Domain.ValueObjects;

namespace Valitana.Infrastructure.Persistence;

/// <summary>
/// Maps Stock and its value objects for EF.
/// </summary>
public sealed class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.HasKey(stock => stock.Symbol);
        builder.Property(stock => stock.Symbol)
            .HasConversion(symbol => symbol.Value, value => Symbol.From(value));
        builder.Property(stock => stock.CurrentPrice)
            .HasConversion(
                price => price == null ? (decimal?)null : price.Value,
                value => value == null ? null : Price.From(value.Value));
    }
}
