﻿using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models;
using Shared.Events;

namespace Order.API.Consumer
{
    public class StockNotReservedEventConsumer : IConsumer<StockReservedEvent>
    {
        readonly OrderAPIDbContext _orderAPIDbContext;

        public StockNotReservedEventConsumer(OrderAPIDbContext orderAPIDbContext)
        {
            _orderAPIDbContext = orderAPIDbContext;
        }

        public async Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            var order = await _orderAPIDbContext.Orders.FirstOrDefaultAsync(o => o.OrderID == context.Message.OrderId);
            order.OrderStatu = Models.Enums.OrderStatus.Failed;
            await _orderAPIDbContext.SaveChangesAsync();
        }
    }
}
