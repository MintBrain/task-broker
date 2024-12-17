using RabbitMQ.Client;

namespace TaskQueue.Services
{
    public class RabbitMqService : IRabbitMqService
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
            if (_connection == null || !_connection.IsOpen)
            {
                _connection = await _connectionFactory.CreateConnectionAsync();
            }

            if (_channel == null || !_channel.IsOpen)
            {
                _channel = await _connection.CreateChannelAsync();
                await DeclareQueueAsync(_channel);
            }

            return _channel;
        }
        
        private async Task DeclareQueueAsync(IChannel channel)
        {
            // Declare the queue asynchronously
            await channel.QueueDeclareAsync(
                queue: "taskQueue",         // Queue name
                durable: false,             // The queue will survive server restarts
                exclusive: false,           // The queue can be used by multiple consumers
                autoDelete: false,          // The queue won't be deleted when it's no longer in use
                arguments: null             // No custom arguments
            );
        }
    }
}