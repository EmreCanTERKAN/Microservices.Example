using MassTransit;
using Stock.API.Consumers;
using Stock.API.Services;
using Shared;
using Stock.API.Models.Entities;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMassTransit(configurator =>
{
    //consumerý tanýmlamak zorundayýz.
    configurator.AddConsumer<OrderCreatedEventConsumer>();
    configurator.UsingRabbitMq((context, _configurator) =>
    {
        _configurator.Host(builder.Configuration["RabbitMQ"]);
        //rabbitmq tarafýndan hangi consumerýn dinleneceðini bildirmek zorundayýz.
        //consumerýn hangi parametresini dinlediðini endpoint üzerinden belirteceðiz.
        _configurator.ReceiveEndpoint(RabbitMQSettings.Stock_OrderCreatedEventQueue, e => e.ConfigureConsumer<OrderCreatedEventConsumer>(context));

    });
});

builder.Services.AddSingleton<MongoDbService>();


#region Harici- MongoDB'ye Seed Data Ekleme
var scope = builder.Services.BuildServiceProvider().CreateScope();

// Servis talep ediyoruz ve referans ile iþaretliyoruz
MongoDbService mongoDBService = scope.ServiceProvider.GetService<MongoDbService>();

// Koleksiyonu talep ediyoruz.Ve stock entitiesi istiyoruz.
var collection = mongoDBService.GetCollection<Stock.API.Models.Entities.Stock>();

var cursor = collection.FindSync<Stock.API.Models.Entities.Stock>(session => true).Any();
// Bundan sonra kontrolde bulunmamýz gerekiyor.. Bu koleksiyonda herhangi bir veri var mý yoksa þunu þunu yap.. 
if (!collection.FindSync<Stock.API.Models.Entities.Stock>(session => true).Any())
{
    await collection.InsertOneAsync(new() { ProductId = Guid.NewGuid(), Count = 2000 });
    await collection.InsertOneAsync(new() { ProductId = Guid.NewGuid(), Count = 1000 });
    await collection.InsertOneAsync(new() { ProductId = Guid.NewGuid(), Count = 3000 });
    await collection.InsertOneAsync(new() { ProductId = Guid.NewGuid(), Count = 5000 });
    await collection.InsertOneAsync(new() { ProductId = Guid.NewGuid(), Count = 500 });
}

#endregion





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
