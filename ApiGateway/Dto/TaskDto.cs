namespace ApiGateway.Dto
{
    public sealed class TaskDto
    {
        public required string Id { get; set; }
        public required string Type { get; set; }
        public required string Data { get; set; }
        public required int TTL { get; set; }
    }

}