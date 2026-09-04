using Microsoft.AspNetCore.SignalR;

namespace Valitana.SignalR;

/// <summary>
/// Broadcast-only hub. Clients never call server methods; the RabbitMQ consumer
/// pushes PriceUpdated to all connected clients.
/// </summary>
public sealed class PriceHub : Hub;
