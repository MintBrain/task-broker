namespace TaskExecutor.Models
{
    public class TaskResult
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string Result { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}