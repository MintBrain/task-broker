namespace ApiGateway.Dto
{
    public class TaskDto
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }
        public int TTL { get; set; }
    }

}