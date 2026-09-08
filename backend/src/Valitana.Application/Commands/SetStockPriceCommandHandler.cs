using MediatR;
using Valitana.Application.Abstractions;
using Valitana.Domain.Aggregates;
using Valitana.Domain.Time;
using Valitana.Domain.ValueObjects;

namespace Valitana.Application.Commands;

public sealed class SetStockPriceCommandHandler(IStockRepository stockRepository, IClock clock)
    : IRequestHandler<SetStockPriceCommand>
{
    public async Task Handle(SetStockPriceCommand request, CancellationToken cancellationToken)
    {
        var symbol = Symbol.From(request.Symbol);
        var price = Price.From(request.Price);

        var stock = stockRepository.GetStock(symbol) ?? new Stock(symbol);
        stock.RecordPrice(price, clock.UtcNow);
        await stockRepository.Upsert(stock, cancellationToken);
    }
}
