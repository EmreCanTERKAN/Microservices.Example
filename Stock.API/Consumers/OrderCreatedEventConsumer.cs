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
        public OrderCreatedEventConsumer(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
            _stockCollection = mongoDbService.GetCollection<Stock.API.Models.Entities.Stock>();
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
            }
            else
            {
                //Siparişin geçersiz tutarsız olduğnua dair işlemler
            }

            return Task.CompletedTask;
        }
    }
}
