using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using TaskQueue.Database;
using TaskQueue.Repositories;
using TaskQueue.Services;
using Microsoft.EntityFrameworkCore;
// using Prometheus;

namespace TaskQueue
{
    public class Startup(IConfiguration configuration)
    {
        
        public IConfiguration Configuration { get; } = configuration;
        
        public void ConfigureServices(IServiceCollection services)
        {
            // Использование строки подключения к базе данных
            // Подключение к PostgreSQL
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddSingleton<IRabbitMqService, RabbitMqService>();

            // Регистрация репозитория
            services.AddScoped<TaskRepository>();

            // Регистрация сервисов
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
            //app.UseSwaggerUI();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskQueue API V1");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}