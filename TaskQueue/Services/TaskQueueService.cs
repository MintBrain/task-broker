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
                Ttl = 3600,
                Status = TaskStatus.New,
                Result = ""
            };
            
            var message = JsonConvert.SerializeObject(_task);
            var body = Encoding.UTF8.GetBytes(message);
            var ch = await _rabbitMqService.GetChannelAsync();
            await ch.BasicPublishAsync(
                exchange: string.Empty, 
                routingKey: "taskQueue", 
                body: body);
        }
        //
        // public void RestartTask(string id)
        // {
        //     // Логика перезапуска задачи
        // }
        //
        // public Task GetTaskById(string id)
        // {
        //     // Логика получения задачи по ID
        //     return new Task(); // Пример возврата
        // }
        //
        // public string GetTaskStatus(string id)
        // {
        //     // Логика получения статуса задачи
        //     return "Статус"; // Пример возврата
        // }
        //
        // public object GetMetrics()
        // {
        //     // Логика получения метрик
        //     return new { }; // Пример возврата метрик
        // }
    }
}