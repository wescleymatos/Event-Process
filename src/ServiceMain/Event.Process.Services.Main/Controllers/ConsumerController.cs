using Event.Process.Services.Main.Contracts;
using Event.Process.Services.Main.RabbitMQ;
using Microsoft.AspNetCore.Mvc;

namespace Event.Process.Services.Main.Controllers;

[ApiController]
[Route("[controller]")]
public class ConsumerController : ControllerBase
{
    private readonly ILogger<ConsumerController> _logger;
    private readonly ConsumerManager _consumerManager;

    public ConsumerController(ConsumerManager consumerManager, ILogger<ConsumerController> logger)
    {
        _consumerManager = consumerManager;
        _logger = logger;
    }

    [HttpGet()]
    public async Task<IEnumerable<Consumer>> GetConsumer()
    {
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
