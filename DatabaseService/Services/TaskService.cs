using DatabaseService.Repositories;
using Shared.Models;

namespace DatabaseService.Services
{
    public class TaskService
    {
        private readonly TaskRepository _taskRepository;

        public TaskService(TaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task AddTask(TaskModel task)
        {
            await _taskRepository.AddTask(task);
        }

        public async Task<TaskModel> GetTask(int id)
        {
            return await _taskRepository.GetTaskById(id);
        }

        // Другие методы для обработки задач
    }
}