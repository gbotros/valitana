using Microsoft.Extensions.Configuration;

namespace Valitana.IntegrationTests;

/// <summary>
/// Loads URLs and probes /health once so tests skip when the stack is down.
/// </summary>
internal static class LiveStack
{
    private static readonly Lazy<Status> State = new(Probe, LazyThreadSafetyMode.ExecutionAndPublication);

    internal static string ApiBaseUrl => State.Value.ApiBaseUrl;

    internal static string SignalRBaseUrl => State.Value.SignalRBaseUrl;

    internal static string? ApiSkipReason => State.Value.ApiSkipReason;

    internal static string? SignalRSkipReason => State.Value.SignalRSkipReason;

    private static Status Probe()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var api = TrimBase(config["ApiBaseUrl"] ?? "http://localhost:5000");
        var signalR = TrimBase(config["SignalRBaseUrl"] ?? "http://localhost:5001");

        return new Status(
            api,
            signalR,
            IsHealthy(api) ? null : $"API not running at {api}. Start the stack (docker compose up) or set ApiBaseUrl.",
            IsHealthy(signalR) ? null : $"SignalR not running at {signalR}. Start the stack or set SignalRBaseUrl.");
    }

    private static string TrimBase(string url)
    {
        return url.TrimEnd('/');
    }

    private static bool IsHealthy(string baseUrl)
    {
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
            var response = client.GetAsync($"{baseUrl}/health").GetAwaiter().GetResult();
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private sealed record Status(
        string ApiBaseUrl,
        string SignalRBaseUrl,
        string? ApiSkipReason,
        string? SignalRSkipReason);
}

public sealed class LiveApiFactAttribute : FactAttribute
{
    public LiveApiFactAttribute()
    {
        if (LiveStack.ApiSkipReason is { } reason)
        {
            Skip = reason;
        }
    }
}

public sealed class LivePipelineFactAttribute : FactAttribute
{
    public LivePipelineFactAttribute()
    {
        if (LiveStack.ApiSkipReason is { } api)
        {
            Skip = api;
        }
        else if (LiveStack.SignalRSkipReason is { } signalR)
        {
            Skip = signalR;
        }
    }
}
