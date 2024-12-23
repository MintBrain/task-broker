using Shared.Enums;
using TaskStatus = Shared.Enums.TaskStatus;

namespace Shared.Models
{
    public class TaskItem
    {
        public int Id { get; set; }      // Идентификатор задачи
        public TaskType Type { get; set; }    // Тип задания
        public string Data { get; set; }    // Исходные данные задания
        public int Ttl { get; set; }        // Время жизни задания в миллисекундах
        public TaskStatus Status { get; set; }  // Статус выполнения
        public string Result { get; set; } // Результат выполнения
    }
}