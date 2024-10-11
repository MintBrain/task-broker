using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Shared.Models;

namespace Shared.Services
{
    public class DatabaseServiceClient
    {
        private readonly HttpClient _httpClient;

        public DatabaseServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<TaskModel>> GetAllTaskStatusesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<TaskModel>>("api/tasks");
        }

        public async Task<TaskModel> GetTaskStatusByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<TaskModel>($"api/tasks/{id}");
        }

        public async Task<TaskModel> SaveTaskResultAsync(TaskModel task)
        {
            return await _httpClient.PostAsJsonAsync<TaskModel>("api/tasks", task);
        }
    }
}