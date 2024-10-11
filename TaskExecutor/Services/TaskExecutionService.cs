using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TaskExecutor.Models;
using Shared.Models;
using Shared.Services; // Убедитесь, что путь соответствует вашему проекту

namespace TaskExecutor.Services
{
    public class TaskExecutionService : BackgroundService
    {
        private readonly ILogger<TaskExecutionService> _logger;
        private readonly DatabaseServiceClient _databaseServiceClient;

        public TaskExecutionService(ILogger<TaskExecutionService> logger, DatabaseServiceClient databaseServiceClient)
        {
            _logger = logger;
            _databaseServiceClient = databaseServiceClient;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Запускаем прослушивание очереди задач из TaskQueue
            Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    // Получение задачи из TaskQueue
                    // var taskItem = await _databaseServiceClient.GetNextTaskAsync(); // Метод получения следующей задачи
                    //
                    // if (taskItem != null)
                    // {
                    //     await ProcessTask(taskItem);
                    // }

                    // Задержка для предотвращения избыточной загрузки
                    await Task.Delay(1000, stoppingToken);
                }
            }, stoppingToken);

            return Task.CompletedTask;
        }

        private async Task ProcessTask(TaskModel taskItem)
        {
            try
            {
                _logger.LogInformation($"Processing task {taskItem.Id}: {taskItem.Description}");

                // Здесь добавьте вашу логику выполнения задачи

                // Сохранение результата в БД через DatabaseServiceClient
                var taskResult = new TaskResult
                {
                    TaskId = taskItem.Id,
                    Result = "Task completed successfully", // Здесь можно добавить результат выполнения задачи
                    CompletedAt = DateTime.UtcNow
                };

                await _databaseServiceClient.SaveTaskResultAsync(taskResult); // Метод для сохранения результата
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing task {TaskId}", taskItem.Id);
                // Обработка ошибки, если нужно
            }
        }
    }
}
