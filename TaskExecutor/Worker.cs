using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using TaskExecutor.Services;

namespace TaskExecutor
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly TaskExecutionService _taskExecutionService;

        public Worker(ILogger<Worker> logger, TaskExecutionService taskExecutionService)
        {
            _logger = logger;
            _taskExecutionService = taskExecutionService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // while (!stoppingToken.IsCancellationRequested)
            // {
            //     if (_logger.IsEnabled(LogLevel.Information))
            //     {
            //         // _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //     }
            // }
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await _taskExecutionService.StartAsync();
        }
    }
}