using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Models;
using System.Text;
using Newtonsoft.Json;
using DatabaseService.Services;

namespace TaskQueue.Services
{
    public class TaskQueueService
    {
        private readonly TaskService _taskService;

        public TaskQueueService(TaskService taskService)
        {
            _taskService = taskService;
        }

        public void StartListening()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "task_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var task = JsonConvert.DeserializeObject<TaskModel>(message);

                // Обрабатываем задачу
                await _taskService.AddTask(task);
                // Логика для выполнения задачи
            };

            channel.BasicConsume(queue: "task_queue", autoAck: true, consumer: consumer);
        }
    }
}