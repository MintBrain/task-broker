namespace TaskExecutor.Models
{
    public class TaskResult
    {
        public string Id { get; set; }          // Идентификатор задачи
        public string Status { get; set; }      // Статус выполнения (успех, ошибка, истек TTL)
        public string Result { get; set; }      // Результат выполнения
    }
}