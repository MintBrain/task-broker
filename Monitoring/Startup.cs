using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration; // Добавлено
using Microsoft.AspNetCore.Hosting;
using Shared.Services; // Добавлено

namespace Monitoring
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
            // Настройка Entity Framework для работы с PostgreSQL
            services.AddHttpClient<DatabaseServiceClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5000/"); // URL вашего DatabaseService
            });

            // Добавление контроллеров
            services.AddControllers();

            // Регистрация Monitoring
            services.AddScoped<Services.MonitoringService>();
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
            });
        }
    }
}