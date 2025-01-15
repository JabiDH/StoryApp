using Serilog;
using Microsoft.OpenApi.Models;
using StoryApp.API.Middlewares;
using StoryApp.API.Services;

namespace StoryApp.API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureLogging(IHostBuilder hostBuilder)
        {
            // Set up Serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Infinite)
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(_configuration)
                .CreateLogger();

            hostBuilder.UseSerilog();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add services to the container
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "StoryApp.API", Version = "v1" });
            });

            services.AddHttpClient();
            services.AddMemoryCache();

            services.AddScoped<ICacheBackService, CacheBackService>();
            services.AddScoped<IStoriesService, StoriesService>();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });
        }

        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseCors();

            app.MapControllers();
        }
    }
}
