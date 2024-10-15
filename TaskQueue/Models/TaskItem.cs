namespace TaskQueue.Models
{
    public class TaskItem
    {
        public string Id { get; set; } // Идентификатор задачи
        public string Type { get; set; } // Тип задания
        public string Data { get; set; } // Исходные данные задания
        public int TTL { get; set; } // Время жизни задания
        public string Status { get; set; } // Статус выполнения
        public string Result { get; set; } // Результат выполнения
    }
}