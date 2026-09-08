using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Valitana.Contracts;

namespace Valitana.SignalR;

/// <summary>
/// Pushes StockPriceUpdated ticks to SignalR clients.
/// </summary>
public sealed class StockPriceUpdatedConsumer(
    IHubContext<PriceHub> hubContext,
    ILogger<StockPriceUpdatedConsumer> logger) : IConsumer<StockPriceUpdated>
{
    public async Task Consume(ConsumeContext<StockPriceUpdated> context)
    {
        await hubContext.Clients.All.SendAsync(
            PriceHubMethods.PriceUpdated, context.Message, context.CancellationToken);
        logger.LogInformation(
            "Pushed {Symbol} {Price} to clients", context.Message.Symbol, context.Message.Price);
    }
}
