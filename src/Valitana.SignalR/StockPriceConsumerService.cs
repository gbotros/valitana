using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Valitana.Contracts;

namespace Valitana.SignalR;

/// <summary>
/// Consumes StockPriceUpdated integration events from RabbitMQ and pushes them
/// to all SignalR clients.
/// </summary>
public sealed class StockPriceConsumerService(
    IHubContext<PriceHub> hubContext,
    IConfiguration configuration,
    ILogger<StockPriceConsumerService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMq:HostName"] ?? "localhost",
            Port = int.TryParse(configuration["RabbitMq:Port"], out var port) ? port : 5672,
            UserName = configuration["RabbitMq:UserName"] ?? "guest",
            Password = configuration["RabbitMq:Password"] ?? "guest",
            AutomaticRecoveryEnabled = true,
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ConsumeAsync(factory, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "RabbitMQ consumer failed, retrying in 5 seconds...");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private async Task ConsumeAsync(ConnectionFactory factory, CancellationToken stoppingToken)
    {
        await using var connection = await factory.CreateConnectionAsync(stoppingToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.ExchangeDeclareAsync(
            MessagingConventions.Exchange, ExchangeType.Topic, durable: true, autoDelete: false,
            cancellationToken: stoppingToken);
        await channel.QueueDeclareAsync(
            MessagingConventions.Queue, durable: true, exclusive: false, autoDelete: false,
            cancellationToken: stoppingToken);
        await channel.QueueBindAsync(
            MessagingConventions.Queue, MessagingConventions.Exchange, MessagingConventions.RoutingKey,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            try
            {
                var priceUpdated = JsonSerializer.Deserialize<StockPriceUpdated>(eventArgs.Body.Span);
                if (priceUpdated is not null)
                {
                    await hubContext.Clients.All.SendAsync(
                        PriceHubMethods.PriceUpdated, priceUpdated, stoppingToken);
                    logger.LogInformation(
                        "Pushed {Symbol} {Price} to clients", priceUpdated.Symbol, priceUpdated.Price);
                }

                await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process message, nacking without requeue");
                await channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false, stoppingToken);
            }
        };

        await channel.BasicConsumeAsync(
            MessagingConventions.Queue, autoAck: false, consumer, cancellationToken: stoppingToken);

        logger.LogInformation("Consuming {Queue}", MessagingConventions.Queue);

        // Keep the consumer alive; automatic recovery handles broker restarts.
        await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
    }
}
