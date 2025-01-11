using System.ComponentModel.DataAnnotations;
using Shared.Enums;

namespace ApiGateway.Dto
{
    public sealed class TaskDto
    {
        [Required(ErrorMessage = "Type is required.")]
        public TaskType Type { get; set; }
    
        [Required(ErrorMessage = "Data is required.")]
        public string Data { get; set; }

        public int Ttl { get; set; } = 0;    // Время жизни задания в миллисекундах
    }

}