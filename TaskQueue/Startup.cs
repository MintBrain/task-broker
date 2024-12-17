using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using RabbitMQ.Client;
using TaskQueue.Services;

// using Prometheus;

namespace TaskQueue
{
    public class Startup(IConfiguration configuration)
    {
        
        public IConfiguration Configuration { get; } = configuration;
        
        public void ConfigureServices(IServiceCollection services)
        {
            // Использование строки подключения к базе данных
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            
            services.AddSingleton<IRabbitMqService, RabbitMqService>();

            // Register TaskQueueService as Scoped
            services.AddScoped<TaskQueueService>();

            services.AddSingleton<IConfiguration>(Configuration);

            // Call asynchronous configuration method for RabbitMQ setup
            // services.AddSingleton<Func<Task>>(async () => await ConfigureRabbitMq(services));
            
            // Использование пользовательских настроек
            var defaultTTL = Configuration.GetValue<int>("TaskSettings:DefaultTTL");
            
            
            // Добавление необходимых сервисов для работы с контроллерами и зависимостями
            services.AddControllers();
            // Настройка подключения к базе данных, кэшу и т.д. будет добавлена здесь

            // Добавление метрик Prometheus
            // services.AddHttpMetrics(); // Сбор HTTP метрик
            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "TaskQueue API", Version = "v1" });
                option.EnableAnnotations(); 
            });

            services.AddMetrics(); // Добавление поддержки метрик
        }

        public void Configure(IApplicationBuilder app)
        {
            // Настройка маршрутизации и обработки запросов
            app.UseRouting();

            // app.UseHttpMetrics();
            // app.UseMetricServer();

            app.UseEndpoints(endpoints =>
            {
                // Подключение маршрутов контроллеров
                endpoints.MapControllers();
                // endpoints.MapMetrics();
                
            });
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }
}