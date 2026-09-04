using MediatR;
using Valitana.Application.Abstractions;
using Valitana.Application.DomainEvents;
using Valitana.Domain.ValueObjects;

namespace Valitana.Application.Commands;

public sealed class SetStockPriceCommandHandler(
    IStockRepository stockRepository,
    IPriceHistoryStore priceHistoryStore,
    IPublisher publisher) : IRequestHandler<SetStockPriceCommand>
{
    public async Task Handle(SetStockPriceCommand request, CancellationToken cancellationToken)
    {
        var symbol = Symbol.From(request.Symbol);
        var price = Price.From(request.Price);
        var occurredAtUtc = DateTimeOffset.UtcNow;

        var stock = stockRepository.GetOrAdd(symbol);
        stock.RecordPrice(price, occurredAtUtc);
        stockRepository.Save(stock);

        priceHistoryStore.Append(new PriceTick(symbol.Value, price.Value, occurredAtUtc));

        foreach (var domainEvent in stock.DequeueDomainEvents())
        {
            await publisher.Publish(DomainEventNotification.From(domainEvent), cancellationToken);
        }
    }
}
