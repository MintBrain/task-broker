using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration; // Добавлено
using Microsoft.AspNetCore.Hosting;
using TaskExecutor.Services;
using Shared.Services; // Убедитесь, что путь соответствует вашему проекту
using Prometheus;

namespace TaskExecutor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Добавление DatabaseService
            services.AddScoped<DatabaseServiceClient>();

            // Добавление TaskExecutionService
            services.AddScoped<TaskExecutionService>();

            // Добавление контроллера
            services.AddControllers();

            services.AddPrometheusCounters();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapMetrics();
            });
        }
    }
}