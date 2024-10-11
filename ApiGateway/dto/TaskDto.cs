namespace ApiGateway.Dto
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int Ttl { get; set; } // Time-to-Live
    }
}