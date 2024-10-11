using Microsoft.AspNetCore.Mvc;
using Monitoring.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Monitoring.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MonitoringController : ControllerBase
    {
        private readonly MonitoringService _monitoringService;

        public MonitoringController(MonitoringService monitoringService)
        {
            _monitoringService = monitoringService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TaskStatus>>> GetAll()
        {
            var statuses = await _monitoringService.GetAllTaskStatusesAsync();
            return Ok(statuses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskStatus>> GetById(int id)
        {
            var status = await _monitoringService.GetTaskStatusByIdAsync(id);
            return Ok(status);
        }
    }
}