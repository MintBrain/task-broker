using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Shared.Models;
using Shared.Services;

namespace Monitoring.Services
{
    public class MonitoringService
    {
        private readonly ILogger<MonitoringService> _logger;
        private readonly DatabaseServiceClient _databaseServiceClient;

        public MonitoringService(ILogger<MonitoringService> logger, DatabaseServiceClient databaseServiceClient)
        {
            _logger = logger;
            _databaseServiceClient = databaseServiceClient;
        }

        public async Task<List<TaskModel>> GetAllTaskStatusesAsync()
        {
            // Получение всех статусов задач из DatabaseService
            return await _databaseServiceClient.GetAllTaskStatusesAsync();
        }

        public async Task<TaskModel> GetTaskStatusByIdAsync(int id)
        {
            // Получение статуса задачи по ID из DatabaseService
            return await _databaseServiceClient.GetTaskStatusByIdAsync(id);
        }
    }
}