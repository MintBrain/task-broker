using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TaskQueue
{
    public class Startup
    {
        
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            // Использование строки подключения к базе данных
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            // Использование настроек RabbitMQ
            var rabbitMqHost = Configuration["RabbitMQ:Host"];
            var rabbitMqQueueName = Configuration["RabbitMQ:QueueName"];
            
            // Использование пользовательских настроек
            var defaultTTL = Configuration.GetValue<int>("TaskSettings:DefaultTTL");
            
            // Добавление необходимых сервисов для работы с контроллерами и зависимостями
            services.AddControllers();
            // Настройка подключения к базе данных, кэшу и т.д. будет добавлена здесь
        }

        public void Configure(IApplicationBuilder app)
        {
            // Настройка маршрутизации и обработки запросов
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                // Подключение маршрутов контроллеров
                endpoints.MapControllers();
            });
        }
    }
}