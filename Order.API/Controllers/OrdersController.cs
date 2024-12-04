using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.API.Models;
using Order.API.Models.Entites;
using Order.API.ViewModels;
using Shared.Events;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderAPIDbContext _context;
        //event için
        private readonly IPublishEndpoint _publishEndPoint;

        public OrdersController(OrderAPIDbContext context, IPublishEndpoint publishEndPoint)
        {
            _context = context;
            _publishEndPoint = publishEndPoint;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderViewModel model)
        {
            Order.API.Models.Entites.Order order = new()
            {
                OrderID = Guid.NewGuid(),
                BuyerID = model.BuyerID,
                CreatedDate = DateTime.Now,
                OrderStatu = Models.Enums.OrderStatus.Suspend
            };
            order.OrderItems = model.OrderItems.Select(oi => new OrderItem
            {
                Count = oi.Count,
                Price = oi.Price,
                ProductId = oi.ProductId,


            }).ToList();
            order.TotalPrice = model.OrderItems.Sum(oi => (oi.Price * oi.Count));
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            OrderCreatedEvent orderCreatedEvent = new()
            {
                BuyerId = order.BuyerID,
                OrderId = order.OrderID,
                OrderItems = order.OrderItems.Select(oi => new Shared.Messages.OrderItemMessage
                {
                    Count = oi.Count,
                    ProductId = oi.ProductId,
                }).ToList()
            };

            await _publishEndPoint.Publish(orderCreatedEvent);

            return Ok(); 
        }
    }
}
