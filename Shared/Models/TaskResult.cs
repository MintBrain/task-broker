using TaskStatus = Shared.Enums.TaskStatus;

namespace Shared.Models
{
    public class TaskResult
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public TaskStatus Status { get; set; }
        public string Result { get; set; }
    }
}