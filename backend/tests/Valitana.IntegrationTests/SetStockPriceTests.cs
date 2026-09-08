using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.SignalR.Client;
using Valitana.Contracts;

namespace Valitana.IntegrationTests;

public sealed class SetStockPriceTests
{
    [LivePipelineFact]
    public async Task Post_records_price_and_broadcasts_PriceUpdated()
    {
        var symbol = $"ZT{Guid.NewGuid():N}"[..10].ToUpperInvariant();
        var price = 187.65m;
        var received = new TaskCompletionSource<StockPriceUpdated>(TaskCreationOptions.RunContinuationsAsynchronously);

        await using var connection = new HubConnectionBuilder()
            .WithUrl($"{LiveStack.SignalRBaseUrl}/hubs/prices")
            .Build();

        connection.On<StockPriceUpdated>(PriceHubMethods.PriceUpdated, update =>
        {
            if (update.Symbol == symbol && update.Price == price)
            {
                received.TrySetResult(update);
            }
        });

        await connection.StartAsync();

        using var client = new HttpClient { BaseAddress = new Uri(LiveStack.ApiBaseUrl + "/") };
        var response = await client.PostAsJsonAsync("api/prices", new { symbol, price });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var completed = await Task.WhenAny(received.Task, Task.Delay(TimeSpan.FromSeconds(10)));
        Assert.True(completed == received.Task, "Timed out waiting for PriceUpdated.");

        var update = await received.Task;
        Assert.Equal(symbol, update.Symbol);
        Assert.Equal(price, update.Price);
    }

    [LiveApiFact]
    public async Task Post_returns_400_for_invalid_price()
    {
        using var client = new HttpClient { BaseAddress = new Uri(LiveStack.ApiBaseUrl + "/") };
        var response = await client.PostAsJsonAsync("api/prices", new { symbol = "AAPL", price = 0m });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
