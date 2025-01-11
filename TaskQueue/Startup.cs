using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using TaskQueue.Database;
using TaskQueue.Repositories;
using TaskQueue.Services;

// using Prometheus;

namespace TaskQueue
{
    public class Startup(IConfiguration configuration)
    {
        
        public IConfiguration Configuration { get; } = configuration;
        
        public void ConfigureServices(IServiceCollection services)
        {
            // PrintConfigurationSection(Configuration); // For debugging
            // Использование строки подключения к базе данных
            // Подключение к PostgreSQL
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddSingleton<IRabbitMqService, RabbitMqService>();
            services.AddScoped<TaskRepository>();               // Регистрация репозитория
            services.AddScoped<TaskQueueService>();             // Регистрация сервиса
            services.AddSingleton<IConfiguration>(Configuration);
            
            // services.AddHostedService<RabbitMqBackgroundService>();
            services.AddSingleton<IHostedService, RabbitMqBackgroundService>();
            
            // Добавление необходимых сервисов для работы с контроллерами и зависимостями
            services.AddControllers();

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
            EnsureDatabaseConnected(app.ApplicationServices);
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
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskQueue API V1");
                c.RoutePrefix = string.Empty;
            });
        }
        
        private void EnsureDatabaseConnected(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<AppDbContext>();

            while (true)
            {
                try
                {
                    Console.WriteLine("Attempting to connect to the database...");
                    context.Database.EnsureCreated(); // Создание базы данных, если ее нет
                    Console.WriteLine("Database connected successfully.");
                    break; // Exit the loop once the connection is successful
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while connecting to the database: {ex.Message}");
                    Console.WriteLine("Retrying in 5 seconds...");
                    Thread.Sleep(5000); // Wait for 5 seconds before retrying
                }
            }
        }
        
        private void PrintConfigurationSection(IConfiguration configuration, string parentKey = "")
        {
            foreach (var child in configuration.GetChildren())
            {
                var key = string.IsNullOrEmpty(parentKey) ? child.Key : $"{parentKey}:{child.Key}";

                if (child.GetChildren().Any()) // If there are nested sections
                {
                    // Recursively print nested sections
                    PrintConfigurationSection(child, key);
                }
                else
                {
                    // Print the key and its value to the console
                    Console.WriteLine($"Config: {key} = {child.Value}");
                }
            }
        }
    }
}