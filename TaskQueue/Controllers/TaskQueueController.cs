using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TaskQueue.Dto;
using TaskQueue.Services;

namespace TaskQueue.Controllers
{
    [ApiController]
    [Route("/queue")]
    public class TaskQueueController : ControllerBase
    {
        private readonly TaskQueueService _taskQueueService;

        public TaskQueueController(TaskQueueService taskQueueService)
        {
            _taskQueueService = taskQueueService;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Get a welcome message", 
            Description = "Returns a simple welcome message from the API.")]
        public IActionResult Get()
        {
            return Ok("Welcome to TaskQueue API!");
        }
        
        [HttpPost]
        [SwaggerOperation(
            Summary = "Добавить задачу в очередь", 
            Description = "Валидирует, добавляет задачу в БД и брокер сообщений")]
        public async Task<IActionResult> AddTask([FromBody] TaskItem task)
        {
            await _taskQueueService.AddTask(task);
            if (task == null)  // TODO: Тут нужно добавить валидацию 
                return BadRequest("Invalid task data");

            try
            {
                var taskId = await _taskQueueService.AddTask(task);
                return Ok(taskId);
            }
            catch (Exception e) // TODO: Handle various exceptions for DB, RabbitMq, ...
            {
                Console.WriteLine(e); // TODO: Store in Log
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        
        [HttpPost("restart/{id:int}")]
        [SwaggerOperation(Summary = "Перезапустить задачу")]
        public async Task<IActionResult> RestartTask(int id)
        {
            try
            {
                await _taskQueueService.RestartTask(id);
                return Ok($"Задача {id} перезапущена");
            }
            catch (Exception e) // TODO: Handle exception for wrong ID 
            {
                Console.WriteLine(e); // TODO: Store in Log
                return StatusCode(StatusCodes.Status500InternalServerError); 
            }
        }

        [HttpGet("result")]
        [SwaggerOperation(Summary = "Получить результат выполнения задачи")]
        public async Task<IActionResult> SendTaskResult([FromBody] int id)
        {
            try
            {
                var result = await _taskQueueService.GetTaskResultById(id);
                return Ok(result);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound($"Task {id} not found");
            }
            catch (Exception e) // TODO: Handle exception for wrong ID 
            {
                Console.WriteLine(e); // TODO: Store in Log
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("tasks")]
        [SwaggerOperation(Summary = "Получить список всех задач")]
        public async Task<IActionResult> GetAllTasks()
        {
            try
            {
                var tasks = await _taskQueueService.GetAllTasks();
                return Ok(tasks);
            }
            catch (Exception e)
            {
                Console.WriteLine(e); // TODO: Store in Log
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("tasks/{id:int}")]
        [SwaggerOperation(Summary = "Получить подробное описание задачи")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            try
            {
                var task = await _taskQueueService.GetTaskById(id);
                return Ok(task);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound($"Task {id} not found");
            }
            catch (Exception e)
            {
                Console.WriteLine(e); // TODO: Store in Log
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("status/{id:int}")]
        [SwaggerOperation(Summary = "Получить статус задачи")]
        public async Task<IActionResult> GetTaskStatus(int id)
        {
            try
            {
                var taskStatus = await _taskQueueService.GetTaskStatus(id);
                return Ok(taskStatus);
            }
            catch (Exception e)
            {
                Console.WriteLine(e); // TODO: Store in Log
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        
        [HttpGet("metrics")]
        [SwaggerOperation(Summary = "Получить метрики TaskQueue")]
        public IActionResult GetMetrics()
        {
            // Пример функции для получения метрик
            // Здесь должна быть логика для извлечения метрик
            return Ok("Метрики"); // Возвращаем метрики
        }
    }
}