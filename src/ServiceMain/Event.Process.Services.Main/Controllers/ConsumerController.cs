using Event.Process.Services.Main.Contracts;
using Event.Process.Services.Main.RabbitMQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Event.Process.Services.Main.Controllers;

[ApiController]
[Route("[controller]")]
public class ConsumerController : ControllerBase
{
    private readonly ILogger<ConsumerController> _logger;
    private readonly ConsumerManager _consumerManager;
    private readonly ServicesMappingConfiguration _serviceMappingConfiguration;
    private readonly IConfiguration _configuration;
    private readonly HttpClientConfiguration _settings;



    public ConsumerController(
        ConsumerManager consumerManager,
        ILogger<ConsumerController> logger,
        IOptions<ServicesMappingConfiguration> serviceMappingConfiguration,
        IConfiguration configuration)
    {
        _consumerManager = consumerManager;
        _logger = logger;
        _serviceMappingConfiguration = serviceMappingConfiguration.Value;
        _configuration = configuration;
        _settings = new HttpClientConfiguration();
    }

    [HttpGet()]
    public async Task<IEnumerable<Consumer>> GetConsumer()
    {
        // var httpConfig = _serviceMappingConfiguration.Services
        //     .Where(x => x.ContainsKey("ServiceOne"))
        //     .Select(x => x.Values.FirstOrDefault())
        //     .FirstOrDefault(x => x.EventName == "EventServiceOne");

        // _configuration.GetSection("EventServiceOneClient").Bind(_settings);

        return await Task.FromResult(_consumerManager.Consumers);
    }

    [HttpPost("init")]
    public async Task<IActionResult> InitConsumer([FromBody] InitConsumerCommand command)
    {
        _consumerManager.AddConsumer(command.QueueName, command.Size, command.MessagesPerSecond);

        return await Task.FromResult(Ok(command));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveConsumer(string id)
    {
        _consumerManager.RemoveConsumer(id);

        return await Task.FromResult(Ok(id));
    }
}
