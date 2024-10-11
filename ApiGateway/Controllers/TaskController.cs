using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ApiGateway.Dto;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Защита всего контроллера
    public class TaskController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public TaskController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Получение задачи по ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"http://localhost:5001/api/tasks/{id}"); // Путь к микросервису
            if (response.IsSuccessStatusCode)
            {
                var taskData = await response.Content.ReadAsStringAsync();
                return Ok(taskData);
            }
            return NotFound();
        }

        // Создание новой задачи
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TaskDto taskDto)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("http://localhost:5001/api/tasks", taskDto); // Путь к микросервису
            if (response.IsSuccessStatusCode)
            {
                return CreatedAtAction(nameof(GetTask), new { id = taskDto.Id }, taskDto);
            }
            return BadRequest();
        }

        // Обновление задачи
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskDto taskDto)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PutAsJsonAsync($"http://localhost:5001/api/tasks/{id}", taskDto);
            if (response.IsSuccessStatusCode)
            {
                return NoContent();
            }
            return NotFound();
        }

        // Удаление задачи
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"http://localhost:5001/api/tasks/{id}");
            if (response.IsSuccessStatusCode)
            {
                return NoContent();
            }
            return NotFound();
        }
    }
}
