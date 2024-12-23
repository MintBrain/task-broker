using System.Text;
using RabbitMQ.Client;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using TaskQueue.Models;
using TaskQueue.Database;
using TaskQueue.Dto;
using Shared.Enums;
using Shared.Models;
using TaskStatus = Shared.Enums.TaskStatus;


namespace TaskQueue.Services
{
    public class TaskQueueService
    {
        private readonly IRabbitMqService _rabbitMqService;
        private readonly AppDbContext _appDbContextContext;
        private int count = 0;

        public TaskQueueService(IRabbitMqService rabbitMqService, AppDbContext appDbContextContext)
        {
            _rabbitMqService = rabbitMqService;
            _appDbContextContext = appDbContextContext;
        }
        
        // Здесь будут методы для работы с задачами
        // Например, добавление задачи в RabbitMQ, получение задач и пр.

        public async Task<int> AddTask(TaskItem task)
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
            
            _appDbContextContext.Tasks.Add(task);  // TODO: Мы должны получать ID из БД
            await _appDbContextContext.SaveChangesAsync();
            
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
            
            return _task.Id;
        }

        public async Task RestartTask(int id)
        {
            // TODO: Store `Restarting` status to DB
            // TODO: Abandon task on rabbitQueue? to drop it in TaskExecutor
            // TODO: Store `RestartFailed` or `RestartSuccess`, get this info from `TaskExecutor` via RabbitMQ
        }
        
        public async Task<TaskItem> GetTaskById(int id)
        {
            // TODO: Get and return TaskItem from DB
            var task = await _appDbContextContext.Tasks.FindAsync(id);
            if (task == null)
                throw new KeyNotFoundException();
            return new TaskItem();
        }

        public async Task<TaskResult> GetTaskResultById(int id)
        {
            var task = await _appDbContextContext.Tasks.FindAsync(id);
            if (task == null)
                throw new KeyNotFoundException("Task not found");

            await _appDbContextContext.SaveChangesAsync();
            return new TaskResult(); // TODO: `TaskResult` может быть не нужен,
                                     // можем возвращать весь TaskItem,
                                     // зависит от того, хранятся ли результаты отдельно в БД
        }

        public async Task<List<TaskItem>> GetAllTasks()
        {
            var tasks = await _appDbContextContext.Tasks.ToListAsync();
            return tasks;
        }
        
        public async Task<TaskStatus> GetTaskStatus(int id)
        {
            var task = await _appDbContextContext.Tasks.FindAsync(id);
            if (task == null)
                throw new KeyNotFoundException();
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
    }
}