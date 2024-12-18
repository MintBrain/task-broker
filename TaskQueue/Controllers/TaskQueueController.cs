using Microsoft.AspNetCore.Mvc;
using TaskQueue.Database;
using TaskQueue.Models;
using Microsoft.EntityFrameworkCore;

namespace TaskQueue.Controllers
{
    [ApiController]
    [Route("queue")]
    public class TaskQueueController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TaskQueueController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddTask([FromBody] TaskItem task)
        {
            if (task == null || task.Id == 0)
                return BadRequest("Invalid task data");

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return Ok("Task added");
        }

        [HttpPost("result")]
        public async Task<IActionResult> ReceiveTaskResult([FromBody] TaskResult result)
        {
            if (result == null || string.IsNullOrEmpty(result.Id))
                return BadRequest("Invalid task result");

            var task = await _context.Tasks.FindAsync(result.Id);
            if (task == null)
                return NotFound($"Task {result.Id} not found");

            task.Status = result.Status;
            task.Result = result.Result;
            await _context.SaveChangesAsync();

            return Ok("Task result updated");
        }

        [HttpGet("tasks")]
        public async Task<IActionResult> GetAllTasks()
        {
            var tasks = await _context.Tasks.ToListAsync();
            return Ok(tasks);
        }

        [HttpGet("tasks/{id}")]
        public async Task<IActionResult> GetTaskById(string id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
                return NotFound($"Task {id} not found");

            return Ok(task);
        }

        [HttpGet("status/{id}")]
        public async Task<IActionResult> GetTaskStatus(string id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
                return NotFound($"Task {id} not found");

            return Ok(new { task.Id, task.Status });
        }
    }
}