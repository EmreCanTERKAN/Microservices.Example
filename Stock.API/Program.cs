using MassTransit;
using Stock.API.Consumers;
using Stock.API.Services;
using Shared;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMassTransit(configurator =>
{
    //consumerı tanımlamak zorundayız.
    configurator.AddConsumer<OrderCreatedEventConsumer>();
    configurator.UsingRabbitMq((context, _configurator) =>
    {
        _configurator.Host(builder.Configuration["RabbitMQ"]);
        //rabbitmq tarafından hangi consumerın dinleneceğini bildirmek zorundayız.
        //consumerın hangi parametresini dinlediğini endpoint üzerinden belirteceğiz.
        _configurator.ReceiveEndpoint(RabbitMQSettings.Stock_OrderCreatedEventQueue, e => e.ConfigureConsumer<OrderCreatedEventConsumer>(context));

    });
});

builder.Services.AddSingleton<MongoDbService>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
