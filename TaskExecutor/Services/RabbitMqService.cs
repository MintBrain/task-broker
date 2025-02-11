using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace TaskExecutor.Services
{
    public class RabbitMqService : IRabbitMqService, IAsyncDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitMqService(IConfiguration configuration)
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:Host"],
                Port = configuration.GetValue<int>("RabbitMQ:Port"),
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"]
            };
        }

        public async Task<IChannel> GetChannelAsync()
        {
            const int retryIntervalInMilliseconds = 5000; // Retry every 5 seconds
            
            while (true)
            {
                try
                {
                    if (_connection == null || !_connection.IsOpen)
                    {
                        Console.WriteLine("Attempting to connect to RabbitMQ...");
                        _connection = await _connectionFactory.CreateConnectionAsync();
                        Console.WriteLine("RabbitMQ connection established.");
                    }

                    if (_channel == null || !_channel.IsOpen)
                    {
                        Console.WriteLine("Creating RabbitMQ channel...");
                        _channel = await _connection.CreateChannelAsync();
                        await DeclareQueueAsync(_channel);
                        Console.WriteLine("RabbitMQ channel created and queues declared.");
                    }

                    return _channel;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"RabbitMQ connection failed: {ex.Message}");
                    Console.WriteLine($"Retrying in {retryIntervalInMilliseconds / 1000} seconds...");

                    // Wait for the retry interval before retrying
                    await Task.Delay(retryIntervalInMilliseconds);
                }
            }
        }

        private static async Task DeclareQueueAsync(IChannel channel)
        {
            if (await QueueDeclarationCheck("taskQueue", channel))
            {
                Console.WriteLine($"Declared queue: \"taskQueue\"");
            }
            else
            {
                Console.WriteLine("Queue undeclared: \"taskQueue\"");
            }

            if (await QueueDeclarationCheck("resultQueue", channel))
            {
                Console.WriteLine($"Declared queue: \"resultQueue\"");
            }
            else
            {
                Console.WriteLine("Queue undeclared: \"resultQueue\"");
            }
        }

        // Generic method to subscribe to any queue
        public async Task SubscribeToQueueAsync(string queueName, Func<string, Task> messageHandler)
        {
            Console.WriteLine($"Subscribing to queue: {queueName}");
            var channel = await GetChannelAsync(); // Get the channel
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                
                await channel.BasicAckAsync(e.DeliveryTag, false);

                await messageHandler(message);
            };

            // Start consuming the messages from the queue
            await channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: false, // Set to true for auto-acknowledgment or false for manual
                consumer: consumer
            );
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null && _channel.IsOpen)
            {
                await _channel.CloseAsync();
                _channel.Dispose();
                _channel = null;
            }

            if (_connection != null && _connection.IsOpen)
            {
                await _connection.CloseAsync();
                _connection.Dispose();
                _connection = null;
            }
        }

        private static async Task<bool> QueueDeclarationCheck(string queueName, IChannel channel)
        {
            var result = await channel.QueueDeclarePassiveAsync(queueName);
            return (!string.IsNullOrEmpty(result.QueueName));
        }
    }
}