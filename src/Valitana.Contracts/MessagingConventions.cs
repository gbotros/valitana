namespace Valitana.Contracts;

/// <summary>RabbitMQ topology shared by publisher and consumer.</summary>
public static class MessagingConventions
{
    public const string Exchange = "valitana.stock";
    public const string Queue = "valitana.stock-price-updated";
    public const string RoutingKey = "stock.price-updated";
}

/// <summary>SignalR method names pushed to browser clients.</summary>
public static class PriceHubMethods
{
    public const string PriceUpdated = "PriceUpdated";
}
