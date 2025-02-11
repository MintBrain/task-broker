using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ApiGateway.Services;
using Prometheus;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using ApiGateway.Common;
using Serilog;

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

        //// Добавление метрик Prometheus
        //services.AddHttpMetrics(); // Сбор HTTP метрик, таких как время ответа и количество запросов

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

        services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {   
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[]{ }
                }
            });
        });
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
        app.UseMiddleware<PrometheusMiddleware>();
        // Принудительное использование HTTPS
        // app.UseHttpsRedirection();

        // Настройка аутентификации
        app.UseAuthentication();
        app.UseRouting();
        app.UseSerilogRequestLogging();

        // Настройка авторизации
        app.UseAuthorization();

        // Добавление метрик Prometheus
        app.UseHttpMetrics(options => options.ReduceStatusCodeCardinality()); // Автоматический сбор HTTP метрик
        app.UseMetricServer(); // Открывает эндпоинт `/metrics` для Prometheus

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapMetrics(); // Добавление метрик к маршрутам
        });
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiGateway API V1");
            c.RoutePrefix = string.Empty;
        });
    }
}
