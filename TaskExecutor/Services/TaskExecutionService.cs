using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Shared.Models;
using TaskStatus = Shared.Enums.TaskStatus;

namespace TaskExecutor.Services
{
    public class TaskExecutionService
    {
        private readonly TaskProcessor _taskProcessor;
        private readonly IRabbitMqService _rabbitMqService;
        private readonly ILogger<TaskExecutionService> _logger; // Логгер

        private const string _taskQueue = "taskQueue";
        private const string _resultQueue = "resultQueue";

        public TaskExecutionService(TaskProcessor taskProcessor, IRabbitMqService rabbitMqService, ILogger<TaskExecutionService> logger)
        {
            _taskProcessor = taskProcessor;
            _rabbitMqService = rabbitMqService;
            _logger = logger; // Инициализация логгера
        }

        public async Task StartAsync()
        {
            // Ensure task and result queues are declared
            var channel = await _rabbitMqService.GetChannelAsync();

            // Логируем начало работы сервиса
            _logger.LogInformation("TaskExecutionService is starting...");

            // Subscribe to the task queue
            await _rabbitMqService.SubscribeToQueueAsync(_taskQueue, async message =>
            {
                var taskItem = JsonConvert.DeserializeObject<TaskItem>(message);

                if (taskItem != null)
                {
                    _logger.LogInformation($"Received task: {taskItem.Id}");
                    await ProcessTaskAsync(taskItem);
                }
                else
                {
                    _logger.LogWarning("Received null task message.");
                }
            });

            _logger.LogInformation("TaskExecutionService is running...");
        }

        private async Task ProcessTaskAsync(TaskItem taskItem)
        {
            var cts = new CancellationTokenSource(taskItem.Ttl);

            try
            {
                // Логируем начало обработки задачи
                _logger.LogInformation($"Processing task {taskItem.Id} with TTL {taskItem.Ttl}ms.");

                // Execute task with a timeout
                double result = await _taskProcessor.ProcessAsync(taskItem, cts.Token);

                // Логируем успешное выполнение задачи
                _logger.LogInformation($"Task {taskItem.Id} completed successfully. Result: {result}");

                // Publish the result to the result queue
                await PublishResultAsync(new TaskResult
                {
                    TaskId = taskItem.Id,
                    Status = TaskStatus.Completed,
                    Result = result.ToString("#0.##", CultureInfo.InvariantCulture)
                });
            }
            catch (OperationCanceledException)
            {
                // Handle TTL expiration
                _logger.LogWarning($"Task {taskItem.Id} expired due to TTL.");
                await PublishResultAsync(new TaskResult
                {
                    TaskId = taskItem.Id,
                    Status = TaskStatus.Expired,
                    Result = "Task expired."
                });
            }
            catch (Exception ex)
            {
                // Handle task failure
                _logger.LogError(ex, $"Task {taskItem.Id} failed with error: {ex.Message}");
                await PublishResultAsync(new TaskResult
                {
                    TaskId = taskItem.Id,
                    Status = TaskStatus.Failed,
                    Result = $"Error: {ex.Message}"
                });
            }
        }

        private async Task PublishResultAsync(TaskResult taskResult)
        {
            var resultMessage = JsonConvert.SerializeObject(taskResult);
            var body = Encoding.UTF8.GetBytes(resultMessage);

            var channel = await _rabbitMqService.GetChannelAsync();

            // Логируем публикацию результата
            _logger.LogInformation($"Publishing result for Task {taskResult.TaskId}: {taskResult.Status}");

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: _resultQueue,
                body: body
            );

            // Логируем успешную публикацию
            _logger.LogInformation($"Result published for Task {taskResult.TaskId}: {taskResult.Status}");
        }
    }
}
