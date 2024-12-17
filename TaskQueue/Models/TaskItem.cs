namespace TaskQueue.Models
{
    public enum TaskType
    {
        Addition = 0,
        Subtraction = 1,
        Multiplication = 2,
        Division = 3,
    }

    public enum TaskStatus
    {
        New = 0,
        Pending = 1,
        InProgress = 2,
        Completed = 3,
        Failed = 4
        // Add other statuses as necessary
    }
    
    public class TaskItem
    {
        public int Id { get; set; }      // Идентификатор задачи
        public TaskType Type { get; set; }    // Тип задания
        public string Data { get; set; }    // Исходные данные задания
        public int Ttl { get; set; }        // Время жизни задания
        public TaskStatus Status { get; set; }  // Статус выполнения
        public string Result { get; set; }  // Результат выполнения
    }
}