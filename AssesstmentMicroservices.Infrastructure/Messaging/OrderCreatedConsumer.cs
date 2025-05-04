using System.Text;
using System.Text.Json;
using AssesstmentMicroservices.Application.Events;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class OrderCreatedConsumer : BackgroundService
{
    private readonly ILogger<OrderCreatedConsumer> _logger;
    private readonly IConfiguration _configuration;
    private IConnection? _connection;
    private IChannel? _channel;
    private const string QueueName = "order_created";

    public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger, IConfiguration configuration)
    {
        _logger = logger;
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
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _logger.LogInformation("Connected to RabbitMQ");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to initialize RabbitMQ connection");
            throw;
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await InitializeRabbitMqAsync();

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (sender, e) =>
        {
            try
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var order = JsonSerializer.Deserialize<OrderCreatedEvent>(message);

                if (order is null)
                {
                    _logger.LogWarning("Received null order message");
                    await _channel.BasicNackAsync(e.DeliveryTag, false, false);
                    return;
                }

                _logger.LogInformation("Processing Order: {OrderId}, Customer: {CustomerId}",
                    order.OrderId, order.CustomerId);

                // Simulasikan proses async
                await Task.Delay(100, stoppingToken);

                await _channel.BasicAckAsync(e.DeliveryTag, false);
                _logger.LogInformation("Order processed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
                await _channel.BasicNackAsync(e.DeliveryTag, false, true); // Requeue pesan
            }
        };

        await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false); // Kontrol aliran pesan

        await _channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer);

        _logger.LogInformation("Consumer started for queue: {QueueName}", QueueName);

        // Tunggu sampai ada permintaan shutdown
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    public override async void Dispose()
    {
        try
        {
            if (_channel?.IsOpen ?? false)
                await _channel.CloseAsync();

            if (_connection?.IsOpen ?? false)
                await _connection.CloseAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during disposal");
        }
        finally
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}