using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Valitana.Application.Abstractions;
using Valitana.Contracts;

namespace Valitana.Infrastructure.Messaging;

/// <summary>
/// Publishes integration events as JSON to the valitana.stock exchange.
/// Connects lazily on first publish and reuses the connection afterwards.
/// </summary>
public sealed class RabbitMqIntegrationEventPublisher(
    IOptions<RabbitMqOptions> options,
    ILogger<RabbitMqIntegrationEventPublisher> logger) : IIntegrationEventPublisher, IAsyncDisposable
{
    private readonly RabbitMqOptions _options = options.Value;
    private readonly SemaphoreSlim _connectLock = new(1, 1);
    private IConnection? _connection;
    private IChannel? _channel;

    public async Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken)
        where TEvent : class
    {
        var channel = await GetChannelAsync(cancellationToken);

        var body = JsonSerializer.SerializeToUtf8Bytes(integrationEvent);
        var properties = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent,
            Type = typeof(TEvent).Name,
        };

        await channel.BasicPublishAsync(
            exchange: MessagingConventions.Exchange,
            routingKey: MessagingConventions.RoutingKey,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);

        logger.LogInformation("Published {EventType} to exchange {Exchange}", typeof(TEvent).Name, MessagingConventions.Exchange);
    }

    private async Task<IChannel> GetChannelAsync(CancellationToken cancellationToken)
    {
        if (_channel is { IsOpen: true })
        {
            return _channel;
        }

        await _connectLock.WaitAsync(cancellationToken);
        try
        {
            if (_channel is { IsOpen: true })
            {
                return _channel;
            }

            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await _channel.ExchangeDeclareAsync(
                exchange: MessagingConventions.Exchange,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken);

            logger.LogInformation("Connected to RabbitMQ at {Host}:{Port}", _options.HostName, _options.Port);
            return _channel;
        }
        finally
        {
            _connectLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
        {
            await _channel.DisposeAsync();
        }

        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }

        _connectLock.Dispose();
    }
}
