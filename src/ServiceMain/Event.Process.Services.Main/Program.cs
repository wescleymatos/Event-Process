using Event.Process.Services.Main.Contracts;
using Event.Process.Services.Main.Extensions;
using Event.Process.Services.Main.RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

var builder = WebApplication.CreateBuilder(args);

/**
 * Add services to the container
 */

//TODO: 
builder.Services.AddSingleton(sp => new ConnectionFactory()
{
    Uri = new Uri("amqp://guest:guest@localhost:5672/test"),
    DispatchConsumersAsync = false,
    ConsumerDispatchConcurrency = 1,
});

//TODO:
builder.Services.AddTransientWithRetry<IConnection, BrokerUnreachableException>(sp => sp.GetRequiredService<ConnectionFactory>().CreateConnection());

//TODO:
builder.Services.AddTransient(sp => sp.GetRequiredService<IConnection>().CreateModel());

builder.Services.AddSingleton<ConsumerManager>();

//TODO: 
builder.Services.AddTransient<Consumer>();

/**
 * Add configure to router lowercase
 */
//TODO: 
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
});

builder.Services.Configure<ServicesMappingConfiguration>(builder.Configuration.GetSection("ServicesMappingConfiguration"));

/* -------------------------------- */

builder.Services.AddControllers(); 

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();