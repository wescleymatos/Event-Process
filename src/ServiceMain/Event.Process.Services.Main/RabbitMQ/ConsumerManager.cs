namespace Event.Process.Services.Main.RabbitMQ;

public class ConsumerManager
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ConsumerManager> _logger;
    private List<Consumer> _consumers = new List<Consumer>();
    private object _syncLock = new();

    public ConsumerManager(IServiceProvider serviceProvider, ILogger<ConsumerManager> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public IEnumerable<Consumer> Consumers => _consumers.ToArray();

    public void AddConsumer(string queue, int size, int messagesPerSecond)
    {
        if (size > 0) {

            for (var i = 1; i <= size; i++)
            {
                var consumer = _serviceProvider.GetRequiredService<Consumer>();
                consumer.Initialize(queue, messagesPerSecond);

                lock (_syncLock)
                {
                    _consumers.Add(consumer);
                    _logger.LogInformation("Consumer {consumerId} added to queue {queue}", consumer.Id, queue);
                }

                consumer.Start();
            }
        }
    }

    public void RemoveConsumer(string id)
        {
            if (_consumers.Count > 0)
            {
                lock (_syncLock)
                {
                    var consumer = _consumers.SingleOrDefault(it => it.Id == id);
                    if (consumer != null)
                    {
                        _consumers.Remove(consumer);
                        consumer.Stop();
                    }
                }
            }
        }
}
