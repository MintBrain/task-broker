using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DatabaseService.Repositories;
using DatabaseService.Services;

namespace DatabaseService
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Настройка базы данных
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql("Host=localhost;Database=taskdb;Username=myusername;Password=mypassword"));

            // Регистрация сервисов и репозиториев
            services.AddScoped<TaskRepository>();
            services.AddScoped<TaskService>();

            // Добавление поддержки контроллеров
            services.AddControllers(); // Регистрация контроллеров
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
                endpoints.MapControllers(); // Регистрация маршрутов контроллеров
            });
        }
    }
}