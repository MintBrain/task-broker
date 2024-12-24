using System.Text;
using RabbitMQ.Client;
using Newtonsoft.Json;
using TaskQueue.Dto;
using Shared.Models;
using TaskQueue.Repositories;
using TaskStatus = Shared.Enums.TaskStatus;


namespace TaskQueue.Services
{
    public class TaskQueueService
    {
        private readonly IRabbitMqService _rabbitMqService;
        private readonly TaskRepository _taskRepository;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public TaskQueueService(IRabbitMqService rabbitMqService, TaskRepository taskRepository,
            IServiceScopeFactory serviceScopeFactory)
        {
            _rabbitMqService = rabbitMqService;
            _taskRepository = taskRepository;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task StartListening()
        {
            // Subscribe to taskQueue
            // await _rabbitMqService.SubscribeToQueueAsync("taskQueue", async message =>
            // {
            //     Console.WriteLine($"Received message from taskQueue: {message}");
            //     // Process the task and handle the message
            // });

            await _rabbitMqService.SubscribeToQueueAsync("resultQueue", async message =>
            {
                try
                {
                    var task = JsonConvert.DeserializeObject<TaskResult>(message);
                    if (task == null) return;

                    Console.WriteLine($"Received result for task {task.TaskId}: message: {message}");

                    using var scope = _serviceScopeFactory.CreateScope();
                    var taskRepository = scope.ServiceProvider.GetRequiredService<TaskRepository>();
                    
                    // Update the task in the database
                    var existingTask = await taskRepository.GetTaskByIdAsync(task.TaskId);
                    if (existingTask != null)
                    {
                        existingTask.Status = task.Status;
                        existingTask.Result = task.Result;
                        await taskRepository.UpdateTaskAsync(existingTask);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error handling result: {ex.Message}");
                }
            });

            // Subscribe to dlq (Dead Letter Queue)
            await _rabbitMqService.SubscribeToQueueAsync("dlq", async message =>
            {
                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var taskRepository = scope.ServiceProvider.GetRequiredService<TaskRepository>();

                    Console.WriteLine($"Dead-lettered message: {message}");
                    var task = JsonConvert.DeserializeObject<TaskItem>(message);
                    if (task == null)
                        return;

                    task.Status = TaskStatus.Expired;
                    await taskRepository.UpdateTaskAsync(task);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }

        public async Task<int> AddTask(TaskDto task)
        {
            var _task = new TaskItem
            {
                Type = task.Type,
                Data = task.Data,
                Ttl = task.Ttl,
                Status = TaskStatus.Pending,
                Result = ""
            };

            _task.Id = (await _taskRepository.AddTaskAsync(_task)).Id;

            await PublishTask(_task);

            return _task.Id;
        }

        public async Task RestartTask(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            if (task == null)
                throw new KeyNotFoundException();

            if (task.Status is TaskStatus.Pending or TaskStatus.InProgress)
            {
                throw new InvalidOperationException($"Task {id} has already been started");
            }

            task.Status = TaskStatus.Pending;
            await _taskRepository.UpdateTaskAsync(task);
            await PublishTask(task);
        }

        public async Task<TaskItem> GetTaskById(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);

            if (task == null)
                throw new KeyNotFoundException();
            return task;
        }

        public async Task<TaskResult> GetTaskResultById(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);

            if (task == null)
                throw new KeyNotFoundException("Task not found");

            return new TaskResult(); // TODO: `TaskResult` может быть не нужен,
            // можем возвращать весь TaskItem,
            // зависит от того, хранятся ли результаты отдельно в БД
        }

        public async Task<List<TaskItem>> GetAllTasks()
        {
            var tasks = await _taskRepository.GetAllTasksAsync();
            return tasks;
        }

        public async Task<TaskStatus> GetTaskStatus(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            if (task == null)
                throw new KeyNotFoundException("Task not found");
            return task.Status;
        }

        public object GetMetrics()
        {
            // TODO: Return Metrics:
            // - Количество поступивших задач (last id in DB)
            // - Количество задач по статусам
            // - Время ожидания выполнения задач (среднее?)
            // - Количество текущих задач в очереди

            return new { }; // Пример возврата метрик
        }

        private async Task PublishTask(TaskItem task)
        {
            var message = JsonConvert.SerializeObject(task);
            var body = Encoding.UTF8.GetBytes(message);
            var ch = await _rabbitMqService.GetChannelAsync();
            var properties = new BasicProperties
            {
                Expiration = (task.Ttl).ToString(),
            };

            await ch.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: "taskQueue",
                mandatory: true,
                basicProperties: properties,
                body: body);
        }
    }
}