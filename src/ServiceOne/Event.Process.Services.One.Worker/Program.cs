using Event.Process.Services.One.Worker;
using MassTransit;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<TestConsumer>(typeof(TestConsumerDefinition));
            // x.AddConsumer<TestDlqConsumer>(typeof(TestDlqConsumerDefinition));
            // x.AddConsumers(typeof(TestConsumer).Assembly);
            x.SetKebabCaseEndpointNameFormatter();
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri("rabbitmq://localhost/test"), h => {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ConfigureEndpoints(context);
                // cfg.ClearSerialization();
                // cfg.UseRawJsonSerializer();
            });
        });

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
