using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using TaskQueue.Database;

namespace TaskQueue.Repositories
{
    public class TaskRepository
    {
        private readonly AppDbContext _dbContext;

        public TaskRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TaskItem> AddTaskAsync(TaskItem task)
        {
            
            var _task = await _dbContext.Tasks.AddAsync(task);
            await _dbContext.SaveChangesAsync();
            return _task.Entity;
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int id)
        {
            return await _dbContext.Tasks.FindAsync(id);
        }

        public async Task<List<TaskItem>> GetAllTasksAsync()
        {
            return await _dbContext.Tasks.ToListAsync();
        }

        public async Task UpdateTaskAsync(TaskItem task)
        {
            // Ensure the entity is being tracked by the context
            var existingTask = await _dbContext.Tasks.FindAsync(task.Id);
            if (existingTask == null)
            {
                throw new KeyNotFoundException($"Task with ID {task.Id} not found.");
            }

            // Update the properties of the existing entity
            _dbContext.Entry(existingTask).CurrentValues.SetValues(task);

            // Save changes to the database
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteTaskAsync(int id)
        {
            var task = await GetTaskByIdAsync(id);
            if (task != null)
            {
                _dbContext.Tasks.Remove(task);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}