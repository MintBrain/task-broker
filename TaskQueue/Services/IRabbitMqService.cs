using RabbitMQ.Client;

namespace TaskQueue.Services
{
    public interface IRabbitMqService
    {
        Task<IChannel> GetChannelAsync();
        
        Task SubscribeToQueueAsync(string queueName, Func<string, Task> messageHandler);
    }
}