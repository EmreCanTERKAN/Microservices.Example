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
    //consumer� tan�mlamak zorunday�z.
    configurator.AddConsumer<OrderCreatedEventConsumer>();
    configurator.UsingRabbitMq((context, _configurator) =>
    {
        _configurator.Host(builder.Configuration["RabbitMQ"]);
        //rabbitmq taraf�ndan hangi consumer�n dinlenece�ini bildirmek zorunday�z.
        //consumer�n hangi parametresini dinledi�ini endpoint �zerinden belirtece�iz.
        _configurator.ReceiveEndpoint(RabbitMQSettings.Stock_OrderCreatedEventQueue, e => e.ConfigureConsumer<OrderCreatedEventConsumer>(context));

    });
});

builder.Services.AddSingleton<MongoDbService>();


#region Harici- MongoDB'ye Seed Data Ekleme
var scope = builder.Services.BuildServiceProvider().CreateScope();

// Servis talep ediyoruz ve referans ile i�aretliyoruz
MongoDbService mongoDBService = scope.ServiceProvider.GetService<MongoDbService>();

// Koleksiyonu talep ediyoruz.Ve stock entitiesi istiyoruz.
var collection = mongoDBService.GetCollection<Stock.API.Models.Entities.Stock>();

var cursor = collection.FindSync<Stock.API.Models.Entities.Stock>(session => true).Any();
// Bundan sonra kontrolde bulunmam�z gerekiyor.. Bu koleksiyonda herhangi bir veri var m� yoksa �unu �unu yap.. 
if (!collection.FindSync<Stock.API.Models.Entities.Stock>(session => true).Any())
{
    await collection.InsertOneAsync(new() { ProductId = "1d6135c3-dabe-4c94-b67d-ff5bc7a28ea2", Count = 2000 });
    await collection.InsertOneAsync(new() { ProductId = "1c860e50-34eb-4909-9e31-519165c3c079", Count = 1000 });
    await collection.InsertOneAsync(new() { ProductId = "7346ed83-e772-45b9-b9a6-a90cf99445fc", Count = 3000 });
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
