using Event.Process.Services.One.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Event.Process.Services.One.Consumer
{
    public class TestEventConsumer : IConsumer<EventDto>
    {
        readonly ILogger<TestEventConsumer> _logger;

        public TestEventConsumer(ILogger<TestEventConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<EventDto> context)
        {
            _logger.LogInformation("Id: {Id}", context.Message.CorrelationId);

            throw new Exception("TestEventConsumer");

            //return Task.CompletedTask;
        }
    }

    public class TestEventConsumerDefinition : ConsumerDefinition<TestEventConsumer>
    {
        private const string EXCHANGE = "service-one-queue";

        public TestEventConsumerDefinition()
        {
            // override the default endpoint name
            EndpointName = EXCHANGE;

            // limit the number of messages consumed concurrently
            // this applies to the consumer only, not the endpoint
            ConcurrentMessageLimit = 8;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<TestEventConsumer> consumerConfigurator)
        {
            // configure message with 3 retries 3 seconds
            endpointConfigurator.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(3)));

            endpointConfigurator.ClearSerialization();
            endpointConfigurator.UseRawJsonSerializer();

            //endpointConfigurator.RethrowFaultedMessages();

            // use the outbox to prevent duplicate events from being published
            endpointConfigurator.UseInMemoryOutbox();
        }
    }
}
