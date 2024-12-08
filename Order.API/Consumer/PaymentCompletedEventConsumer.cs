using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models;
using Order.API.Models.Entites;
using Shared.Events;

namespace Order.API.Consumer
{
    public class PaymentCompletedEventConsumer : IConsumer<PaymentCompletedEvent>
    {
        readonly OrderAPIDbContext _orderAPIDbContext;

        public PaymentCompletedEventConsumer(OrderAPIDbContext orderAPIDbcontext)
        {
            _orderAPIDbContext = orderAPIDbcontext;
        }

        public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            Order.API.Models.Entites.Order order = await _orderAPIDbContext.Orders.FirstOrDefaultAsync(x => x.OrderID == context.Message.OrderId);
            order.OrderStatu = Models.Enums.OrderStatus.Completed;
            await _orderAPIDbContext.SaveChangesAsync();
        }
    }
}

