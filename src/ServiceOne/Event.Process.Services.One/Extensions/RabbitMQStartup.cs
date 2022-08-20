using Event.Process.Services.One.Consumer;
using MassTransit;

namespace Event.Process.Services.One.Extensions
{
    public static class RabbitMQStartup
    {
        public static void AddRabbitmq(this IServiceCollection service)
        {
            service.AddMassTransit(x =>
            {
                //x.AddConsumer<TestConsumer>(typeof(TestConsumerDefinition));
                x.AddConsumer<TestEventConsumer>(typeof(TestEventConsumerDefinition));

                x.SetKebabCaseEndpointNameFormatter();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(new Uri("rabbitmq://localhost/test"), h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });
        }
    }
}
