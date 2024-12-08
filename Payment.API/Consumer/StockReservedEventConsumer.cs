using MassTransit;
using Shared.Events;

namespace Payment.API.Consumer
{
    public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
    {
        private readonly IPublishEndpoint _endpoint;

        public StockReservedEventConsumer(IPublishEndpoint endpoint)
        {
            _endpoint = endpoint;
        }

        public async Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            // Ödeme İşlemleri..

            if (true)
            {
                //Ödemenin başarıyla tamamlandığını ifade etmemiz lazım...
                //Eventi yayınlıyoruz.
                PaymentCompletedEvent paymentCompletedEvent = new()
                {
                    OrderId = context.Message.OrderId
                };
                await context.Publish(paymentCompletedEvent);

                // Süresi daha net takip edebbilmek için ödeme başarılı diyoruz....
                Console.WriteLine("Ödeme Başarılı...");

            }
            else
            {
                // Ödemede  sıkıntı olduğunu...
                PaymentFailedEvent paymentFailedEvent = new()
                {
                    OrderId = context.Message.OrderId,
                    Message = "..."
                };
               await _endpoint.Publish(paymentFailedEvent);
                Console.WriteLine("Ödeme Başarısız....");
            }
           // return Task.CompletedTask;
        }
    }
}
