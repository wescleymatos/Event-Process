using Event.Process.Services.One.Extensions;
using WatchDog;

namespace Event.Process.Services.One
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
            services.AddControllers();

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen();

            services.AddLogging(loggingBuilder => loggingBuilder.AddSeq(Configuration.GetSection("Seq")));

            services.AddRabbitmq();

            services.AddWatchDogServices();

            services.AddDependencyInjection();
        }

        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                // Swagger
                app.UseSwagger();
                app.UseSwaggerUI();

                // Configure WatchDog
                app.UseWatchDogExceptionLogger();
                app.UseWatchDog(opt =>
                {
                    opt.WatchPageUsername = "admin";
                    opt.WatchPagePassword = "1234";
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
        }
    }
}
