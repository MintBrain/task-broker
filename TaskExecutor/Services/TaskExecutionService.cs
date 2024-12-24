using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace TaskExecutor.Services
{
    public class TaskExecutor
    {
        private readonly TaskProcessor _taskProcessor;
        private readonly string _rabbitMqHost;
        private readonly string _queueName = "TaskQueue";

        public TaskExecutor(TaskProcessor taskProcessor, string rabbitMqHost)
        {
            _taskProcessor = taskProcessor;
            _rabbitMqHost = rabbitMqHost;
        }

        public void Start()
        {
            var factory = new ConnectionFactory { HostName = _rabbitMqHost };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: _queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            Console.WriteLine($"Listening on queue '{_queueName}'...");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"Received task: {message}");

                // Обработка задачи
                var result = await _taskProcessor.ProcessAsync(message);

                // Вывод результата
                Console.WriteLine($"Processed task: {result}");
            };

            channel.BasicConsume(queue: _queueName,
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
