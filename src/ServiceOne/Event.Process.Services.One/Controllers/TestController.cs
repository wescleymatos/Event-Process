using Event.Process.Services.One.Contracts;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Event.Process.Services.One.Controllers
{
    [ApiController]
    [Route("test")]
    public class TestController : ControllerBase
    {
        private const string QUEUE = "service-one-queue";
        private const string EXCHANGE = "service-one-queue";

        private readonly ConnectionFactory _connectionFactory;
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
            _connectionFactory = new ConnectionFactory
            {
                Uri = new Uri("amqp://guest:guest@localhost:5672/test")
            };
        }

        [HttpGet("", Name = "GetSuccessAsync")]
        public async Task<IActionResult> GetSuccessAsync()
        {
            return await Task.FromResult(Ok());
        }

        [HttpPost("process-success", Name = "ProcessSuccessAsync")]
        public async Task<IActionResult> ProcessSuccessAsync()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: QUEUE, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.ExchangeDeclare(exchange: EXCHANGE, type: "fanout", arguments: null);
                    channel.QueueBind(queue: QUEUE, exchange: EXCHANGE, routingKey: "");
                    var dto = new Dto
                    {
                        Id = Guid.NewGuid(),
                        Name = "Nome do Cliente",
                        Email = "email@teste.com"
                    };
                    var stringfy = JsonSerializer.Serialize(dto);
                    var byteArray = Encoding.UTF8.GetBytes(stringfy);

                    channel.BasicPublish(exchange: EXCHANGE, routingKey: "", basicProperties: null, body: byteArray);
                }
            }

            return await Task.FromResult(Ok());
        }

        [HttpPost("process-error", Name = "ProcessErrorAsync")]
        public async Task<IActionResult> ProcessErrorAsync()
        {
            return await Task.FromResult(BadRequest());
        }
    }
}