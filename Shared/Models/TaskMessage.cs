namespace Shared.Models
{
    public class TaskMessage
    {
        public string TaskId { get; set; }     // Уникальный идентификатор задачи
        public string TaskType { get; set; }   // Тип задачи (например, обработка данных, отправка email)
        public string TaskData { get; set; }   // Данные для выполнения задачи
        public int TTL { get; set; }           // Время жизни задачи (TTL)
        
        public TaskMessage(string taskId, string taskType, string taskData, int ttl)
        {
            TaskId = taskId;
            TaskType = taskType;
            TaskData = taskData;
            TTL = ttl;
        }
    }
}