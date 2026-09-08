using Microsoft.AspNetCore.SignalR;

namespace Valitana.SignalR;

/// <summary>
/// Broadcast-only; clients do not call server methods.
/// </summary>
public sealed class PriceHub : Hub;
