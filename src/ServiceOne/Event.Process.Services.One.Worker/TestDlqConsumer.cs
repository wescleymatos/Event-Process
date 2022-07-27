using Event.Process.Services.One.Contracts;
using MassTransit;

namespace Event.Process.Services.One.Worker
{
    public class TestDlqConsumer : IConsumer<Dto>
    {
        readonly ILogger<TestDlqConsumer> _logger;

        public TestDlqConsumer(ILogger<TestDlqConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<Dto> context)
        {
            _logger.LogInformation("Id: {Id}", context.Message.Id);

            throw new Exception("TestDlqConsumer");

            //return Task.CompletedTask;
        }
    }

    internal class TestDlqConsumerDefinition : ConsumerDefinition<TestDlqConsumer>
    {
        private const string EXCHANGE = "service-one-queue_error";

        public TestDlqConsumerDefinition()
        {
            // override the default endpoint name
            EndpointName = EXCHANGE;

            // limit the number of messages consumed concurrently
            // this applies to the consumer only, not the endpoint
            ConcurrentMessageLimit = 8;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<TestDlqConsumer> consumerConfigurator)
        {
            // configure message with 3 retries 3 seconds
            endpointConfigurator.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(3)));

            endpointConfigurator.ClearSerialization();
            endpointConfigurator.UseRawJsonSerializer();

            endpointConfigurator.RethrowFaultedMessages();

            // use the outbox to prevent duplicate events from being published
            endpointConfigurator.UseInMemoryOutbox();
        }
    }
}
