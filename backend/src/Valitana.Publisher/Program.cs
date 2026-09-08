using System.Net.Http.Json;

// POSTs a random-walk price to the API every second.

var apiBaseUrl = Environment.GetEnvironmentVariable("ApiBaseUrl") ?? "http://localhost:5000";
var symbol = Environment.GetEnvironmentVariable("Symbol") ?? "AAPL";

using var httpClient = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };

var random = new Random();
var price = 150.00m;

Console.WriteLine($"Posting a new {symbol} price to {apiBaseUrl}/api/prices every second. Ctrl+C to stop.");

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

while (!cts.IsCancellationRequested)
{
    // Random walk: drift by up to +/- 0.5, never below 1.
    var delta = (decimal)(random.NextDouble() - 0.5);
    price = Math.Max(1m, Math.Round(price + delta, 2));

    try
    {
        var response = await httpClient.PostAsJsonAsync(
            "/api/prices",
            new { symbol, price },
            cts.Token);

        Console.WriteLine(response.IsSuccessStatusCode
            ? $"{DateTimeOffset.UtcNow:HH:mm:ss} {symbol} {price}"
            : $"{DateTimeOffset.UtcNow:HH:mm:ss} API returned {(int)response.StatusCode}");
    }
    catch (OperationCanceledException) when (cts.IsCancellationRequested)
    {
        break;
    }
    catch (HttpRequestException ex)
    {
        // API not up yet (compose start order) - keep retrying.
        Console.WriteLine($"{DateTimeOffset.UtcNow:HH:mm:ss} API unreachable ({ex.Message}), retrying...");
    }

    try
    {
        await Task.Delay(TimeSpan.FromSeconds(1), cts.Token);
    }
    catch (OperationCanceledException)
    {
        break;
    }
}

Console.WriteLine("Publisher stopped.");
