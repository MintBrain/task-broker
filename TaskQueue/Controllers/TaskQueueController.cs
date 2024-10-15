using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TaskQueue.Models;

namespace TaskQueue.Controllers
{
    [ApiController]
    [Route("queue")]
    public class TaskQueueController : ControllerBase
    {
        // POST /queue
        [HttpPost]
        public IActionResult AddTask([FromBody] TaskItem task)
        {
            // Пример функции для добавления задачи в очередь
            // Здесь должна быть логика для валидации и добавления задачи в RabbitMQ
            return Ok("Задача добавлена"); // Возвращаем успех
        }

        // POST /queue/restart/{id}
        [HttpPost("restart/{id}")]
        public IActionResult RestartTask(string id)
        {
            // Пример функции для перезапуска задачи по ID
            // Здесь должна быть логика для перезапуска задачи
            return Ok($"Задача {id} перезапущена");
        }
        
        [HttpPost("result")]
        public async Task<IActionResult> ReceiveTaskResult([FromBody] TaskResult result)
        {
            if (result == null || string.IsNullOrEmpty(result.Id))
            {
                return BadRequest("Invalid task result");
            }

            // Обновление задачи в БД по Id
            await _taskService.UpdateTaskResult(result.Id, result.Status, result.Result);

            return Ok();
        }

        // GET /queue/tasks
        [HttpGet("tasks")]
        public IActionResult GetAllTasks()
        {
            // Пример функции для получения всех задач
            // Здесь должна быть логика для извлечения всех задач из базы данных
            return Ok("Список всех задач"); // Возвращаем список задач
        }

        // GET /queue/tasks/{id}
        [HttpGet("tasks/{id}")]
        public IActionResult GetTaskById(string id)
        {
            // Пример функции для получения задачи по ID
            // Здесь должна быть логика для получения задачи по ID
            return Ok($"Задача с ID {id}"); // Возвращаем задачу
        }

        // GET /queue/status/{id}
        [HttpGet("status/{id}")]
        public IActionResult GetTaskStatus(string id)
        {
            // Пример функции для получения статуса задачи по ID
            // Здесь должна быть логика для получения статуса задачи
            return Ok($"Статус задачи {id}"); // Возвращаем статус задачи
        }

        // GET /queue/metrics/
        [HttpGet("metrics")]
        public IActionResult GetMetrics()
        {
            // Пример функции для получения метрик
            // Здесь должна быть логика для извлечения метрик
            return Ok("Метрики"); // Возвращаем метрики
        }
    }
}
