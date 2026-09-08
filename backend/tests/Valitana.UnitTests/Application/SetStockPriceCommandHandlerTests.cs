using Valitana.Application.Commands;
using Valitana.Domain.Aggregates;
using Valitana.Domain.ValueObjects;
using Valitana.UnitTests.Fakes;

namespace Valitana.UnitTests.Application;

public sealed class SetStockPriceCommandHandlerTests
{
    private static readonly DateTimeOffset Now = new(2026, 9, 8, 13, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task Handle_creates_a_stock_when_missing()
    {
        var repository = new FakeStockRepository();
        var handler = new SetStockPriceCommandHandler(repository, new FixedClock(Now));

        await handler.Handle(new SetStockPriceCommand("aapl", 150.25m), CancellationToken.None);

        var stock = Assert.Single(repository.Items.Values);
        Assert.Equal("AAPL", stock.Symbol.Value);
        Assert.Equal(150.25m, stock.CurrentPrice!.Value);
        Assert.Equal(Now, stock.LastUpdatedAtUtc);
        Assert.Equal(1, repository.UpsertCount);
    }

    [Fact]
    public async Task Handle_updates_an_existing_stock()
    {
        var repository = new FakeStockRepository();
        var existing = new Stock(Symbol.From("AAPL"));
        existing.RecordPrice(Price.From(100m), Now.AddMinutes(-1));
        existing.DequeueDomainEvents();
        await repository.Upsert(existing, CancellationToken.None);

        var handler = new SetStockPriceCommandHandler(repository, new FixedClock(Now));
        await handler.Handle(new SetStockPriceCommand("AAPL", 101m), CancellationToken.None);

        var stock = repository.GetStock(Symbol.From("AAPL"));
        Assert.NotNull(stock);
        Assert.Equal(101m, stock.CurrentPrice!.Value);
        Assert.Equal(Now, stock.LastUpdatedAtUtc);
        Assert.Equal(2, repository.UpsertCount);
    }

    [Fact]
    public async Task Handle_stamps_clock_utc_now()
    {
        var repository = new FakeStockRepository();
        var handler = new SetStockPriceCommandHandler(repository, new FixedClock(Now));

        await handler.Handle(new SetStockPriceCommand("AAPL", 10m), CancellationToken.None);

        Assert.Equal(Now, repository.Items["AAPL"].LastUpdatedAtUtc);
    }

    [Theory]
    [InlineData("", 10)]
    [InlineData("AAPL", 0)]
    [InlineData("AAPL", -1)]
    public async Task Handle_rejects_invalid_symbol_or_price_before_upsert(string symbol, decimal price)
    {
        var repository = new FakeStockRepository();
        var handler = new SetStockPriceCommandHandler(repository, new FixedClock(Now));

        await Assert.ThrowsAnyAsync<ArgumentException>(
            () => handler.Handle(new SetStockPriceCommand(symbol, price), CancellationToken.None));

        Assert.Equal(0, repository.UpsertCount);
        Assert.Empty(repository.Items);
    }
}
