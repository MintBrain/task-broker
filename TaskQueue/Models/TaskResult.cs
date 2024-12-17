namespace TaskQueue.Models
{
    public class TaskResult
    {
        public string Id { get; set; }
        public string TaskId { get; set; }
        public string Result { get; set; }
        public string Status { get; set; }
        // Другие поля по необходимости
    }
}