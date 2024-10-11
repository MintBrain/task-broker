namespace Shared.Models
{
    public class TaskModel
    {
        public int Id { get; set; }
        public required string Description { get; set; }
        public int Ttl { get; set; } // Time-to-Live
        public DateTime CreatedAt { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsExpired => DateTime.UtcNow > CreatedAt.AddSeconds(Ttl);
    }
}
