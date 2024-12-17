using System.Text;
using RabbitMQ.Client;
using TaskQueue.Models;
using Newtonsoft.Json;
using TaskStatus = TaskQueue.Models.TaskStatus;


namespace TaskQueue.Services
{
    public class TaskQueueService
    {
        private readonly IRabbitMqService _rabbitMqService;
        private int count = 0;

        public TaskQueueService(IRabbitMqService rabbitMqService)
        {
            _rabbitMqService = rabbitMqService;
        }
        
        // Здесь будут методы для работы с задачами
        // Например, добавление задачи в RabbitMQ, получение задач и пр.

        public async Task AddTask(TaskItem task)
        {
            // TODO: Add task to DB and get ID
            var _task = new TaskItem
            {
                Id = count++,
                Type = TaskType.Addition,
                Data = "[1,3]",
                Ttl = 3600 * 1000,
                Status = TaskStatus.New,
                Result = ""
            };
            
            var message = JsonConvert.SerializeObject(_task);
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

        public void RestartTask(int id)
        {
            // TODO: Store `Restarting` status to DB
            // TODO: Abandon task on rabbitQueue? to drop it in TaskExecutor
            // TODO: Store `RestartFailed` or `RestartSuccess`, get this info from `TaskExecutor` via RabbitMQ
        }
        
        public TaskItem GetTaskById(int id)
        {
            // TODO: Get and return TaskItem from DB

            return new TaskItem();
        }
        
        public TaskStatus GetTaskStatus(int id)
        {
            // Логика получения статуса задачи
            return TaskStatus.New;
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
    }
}