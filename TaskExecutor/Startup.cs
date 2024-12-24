using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskExecutor.Services;

namespace TaskExecutor
{
    public class Startup(IConfiguration configuration)
    {
        public IConfiguration Configuration { get; } = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IRabbitMqService, RabbitMqService>();
            
            services.AddSingleton<TaskProcessor>();

            // Register TaskExecutor
            services.AddSingleton<TaskExecutionService>();

            // Register Hosted Service for lifecycle management
            services.AddHostedService<Worker>();
            services.AddSingleton(Configuration);
        }
    }
}