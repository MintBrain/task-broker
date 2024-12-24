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
        private readonly int _defaultTtl;

        public TaskQueueController(TaskQueueService taskQueueService, IConfiguration configuration)
        {
            _taskQueueService = taskQueueService;
            _defaultTtl = configuration.GetValue<int>("TaskSettings:DefaultTTL");
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
        public async Task<IActionResult> AddTask([FromBody] TaskDto task)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);
            
            // Set default TTL if not provided
            if (task.Ttl <= 0)
            {
                task.Ttl = _defaultTtl;
            }

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
            catch (KeyNotFoundException e)
            {
                return NotFound($"Task {id} not found");
            }
            catch (InvalidOperationException e)
            {
                return BadRequest($"Task {id} has already been started");
            }
            catch (Exception e)
            {
                Console.WriteLine(e); // TODO: Store in Log
                return StatusCode(StatusCodes.Status500InternalServerError); 
            }
        }

        [HttpGet("result/{id:int}")]
        [SwaggerOperation(Summary = "Получить результат выполнения задачи")]
        public async Task<IActionResult> SendTaskResult(int id)
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
            catch (Exception e)
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
        
        [HttpGet("metrics")]
        [SwaggerOperation(Summary = "Получить метрики TaskQueue")]
        public IActionResult GetMetrics()
        {
            try
            {
                var metrics = _taskQueueService.GetMetrics();
                return Ok(metrics);
            }
            catch (Exception e)
            {
                Console.WriteLine(e); // TODO: Store in Log
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}