using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ApiGateway.Services;
using Prometheus;

public class Startup
{
    public IConfiguration Configuration { get; }
    
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Поддержка контроллеров
        services.AddControllers();
        
        // Регистрация AuthService для работы с JWT токенами
        services.AddScoped<IAuthService, AuthService>();
        
        // Настройка аутентификации с использованием JWT
        var key = Encoding.ASCII.GetBytes(Configuration["Jwt:Key"]);
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

        // Добавление метрик
        services.AddMetrics(); // Предполагается существование собственной настройки метрик

        // HTTP клиент для маршрутизации запросов к TaskQueue
        services.AddHttpClient("TaskQueueClient", client =>
        {
            client.BaseAddress = new Uri(Configuration["TaskQueue:BaseUrl"]);
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // Принудительное использование HTTPS
        app.UseHttpsRedirection();

        // Настройка аутентификации
        app.UseAuthentication();

        // Настройка авторизации
        app.UseAuthorization();

        app.UseRouting();
        
        // Добавление метрик
        app.UseMetricServer(); // Включает сервер метрик по умолчанию
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
