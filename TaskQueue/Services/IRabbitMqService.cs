using RabbitMQ.Client;

namespace TaskQueue.Services
{
    public interface IRabbitMqService
    {
        Task<IChannel> GetChannelAsync();
    }
}