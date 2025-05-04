using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Microsoft.Extensions.Configuration;
using AssesstmentMicroservices.Application.Events;
using AssesstmentMicroservices.Application.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata;

public class RabbitMqPublisher : IMessagePublisher, IDisposable
{
    private readonly IConfiguration _configuration;
    private IConnection _connection;
    private IChannel _channel;
    private const string QueueName = "order_created";
    private bool _disposed;

    public RabbitMqPublisher(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private async Task InitializeRabbitMqAsync()
    {
        try
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQ:HostName"] ?? "localhost"
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            
            await _channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to initialize RabbitMQ connection", ex);
        }
    }

    public async Task PublishOrderCreatedAsync(OrderCreatedEvent orderEvent)
    {
        await InitializeRabbitMqAsync();
        if (_disposed)
            throw new ObjectDisposedException(nameof(RabbitMqPublisher));

        try
        {
            var json = JsonSerializer.Serialize(orderEvent);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent,
                Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            };

            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: QueueName,
                mandatory: true,
                basicProperties: properties,
                body: body);

        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to publish message", ex);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        try
        {
            if (_channel?.IsOpen ?? false)
                _channel.CloseAsync();

            if (_connection?.IsOpen ?? false)
                _connection.CloseAsync();

            _channel?.Dispose();
            _connection?.Dispose();
        }
        finally
        {
            _disposed = true;
        }
    }
}