using MassTransit;
using MongoDB.Driver;
using Shared.Events;
using Shared.Messages;
using Stock.API.Services;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        //Veri tabanından stockları kontrol edeceğimiz için mongoDbServisi enjecte etmemiz gerekmektedir.

        readonly MongoDbService _mongoDbService;
        IMongoCollection<Stock.API.Models.Entities.Stock> _stockCollection;
        readonly ISendEndpointProvider _sendEndPointProvider;
        readonly IPublishEndpoint _publishEndpoint;
        public OrderCreatedEventConsumer(MongoDbService mongoDbService, ISendEndpointProvider sendEndPointProvider, IPublishEndpoint publishEndpoint)
        {
            _mongoDbService = mongoDbService;
            _stockCollection = mongoDbService.GetCollection<Stock.API.Models.Entities.Stock>();
            _sendEndPointProvider = sendEndPointProvider;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            List<bool> stockResult = new();
            foreach (OrderItemMessage item in context.Message.OrderItems)
            {
                stockResult.Add((await _stockCollection.FindAsync(s => s.ProductId == item.ProductId && s.Count >= item.Count)).Any());
            }

            if (stockResult.TrueForAll(sr => sr.Equals(true)))
            {
                //Gerekli sipariş işlemleri...
                foreach (OrderItemMessage orderItem in context.Message.OrderItems)
                {

                    Stock.API.Models.Entities.Stock stock = await (await _stockCollection.FindAsync(s => s.ProductId == orderItem.ProductId)).FirstOrDefaultAsync();

                    stock.Count -= orderItem.Count;
                    await _stockCollection.FindOneAndReplaceAsync(s => s.ProductId == orderItem.ProductId, stock);
                }

                //Payment e veri göndereceğiz.Stock işlemlerinin tamamlandığna dair bir event oluşturup Payment servise o eventi göndereceğiz...Paymentta ödeme işlemi yapacağız.

                StockReservedEvent stockReservedEvent = new StockReservedEvent()
                {
                    TotalPrice = context.Message.TotalPrice,
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId,

                };

                var sendEndPoint = await _sendEndPointProvider.GetSendEndpoint(new Uri($"queue:{Shared.RabbitMQSettings.Payment_StockReservedEventQueue}"));
                await sendEndPoint.Send(stockReservedEvent);

                Console.WriteLine("Stok İşlemleri Başarılı...");

            }
            else
            {
                StockNotReservedEvent stockNotReservedEvent = new StockNotReservedEvent()
                {
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId,
                    Message = "..."
                };

                await _publishEndpoint.Publish(stockNotReservedEvent);
                Console.WriteLine("Stok İşlemleri Başarısız...");

            }

           // return Task.CompletedTask;
        }
    }
}
