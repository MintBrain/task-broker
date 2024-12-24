using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TaskExecutor.Services;

namespace TaskExecutor
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Регистрация зависимостей
            services.AddSingleton<TaskProcessor>();
            services.AddSingleton(provider =>
            {
                var taskProcessor = provider.GetRequiredService<TaskProcessor>();
                var rabbitMqHost = "localhost"; // Укажите хост RabbitMQ
                return new TaskExecutor.Services.TaskExecutor(taskProcessor, rabbitMqHost);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, TaskExecutor.Services.TaskExecutor taskExecutor)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Запуск TaskExecutor при старте
            taskExecutor.Start();
        }
    }
}
