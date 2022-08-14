using Event.Process.Services.One.Consumer;
using MassTransit;
using WatchDog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
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

builder.Services.AddWatchDogServices();

var app = builder.Build();

app.UseWatchDog(opt =>
{
    opt.WatchPageUsername = "admin";
    opt.WatchPagePassword = "1234";
});

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
