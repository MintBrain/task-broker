namespace TaskQueue.Services;

public class RabbitMqBackgroundService : BackgroundService
{
    private readonly IRabbitMqService _rabbitMqService;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public RabbitMqBackgroundService(IRabbitMqService rabbitMqService, IServiceScopeFactory serviceScopeFactory)
    {
        _rabbitMqService = rabbitMqService;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Create a scope to resolve the scoped service
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var taskQueueService = scope.ServiceProvider.GetRequiredService<TaskQueueService>();

            // Declare queues and start listening
            await _rabbitMqService.GetChannelAsync();
            await taskQueueService.StartListening();
        }
    }
}
