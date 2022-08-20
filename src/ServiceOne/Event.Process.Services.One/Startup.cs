using Event.Process.Services.One.Consumer;
using Event.Process.Services.One.Extensions;
using MassTransit;
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

            //services.AddMassTransit(x =>
            //{
            //    //x.AddConsumer<TestConsumer>(typeof(TestConsumerDefinition));
            //    x.AddConsumer<TestEventConsumer>(typeof(TestEventConsumerDefinition));

            //    x.SetKebabCaseEndpointNameFormatter();
            //    x.UsingRabbitMq((context, cfg) =>
            //    {
            //        cfg.Host(new Uri("rabbitmq://localhost/test"), h =>
            //        {
            //            h.Username("guest");
            //            h.Password("guest");
            //        });

            //        cfg.ConfigureEndpoints(context);
            //    });
            //});
            services.AddRabbitmq();
            services.AddDependencyInjection();

            // Add WatchDog Service
            services.AddWatchDogServices();
        }

        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            // Configure the HTTP request pipeline.
            //if (env.IsDevelopment())
            //{
                app.UseSwagger();
                app.UseSwaggerUI();
            //}

            // Configure WatchDog
            app.UseWatchDogExceptionLogger();
            app.UseWatchDog(opt =>
            {
                opt.WatchPageUsername = "admin";
                opt.WatchPagePassword = "1234";
            });

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
        }
    }
}
