using Order.API.Models.Enums;

namespace Order.API.Models.Entites
{
    public class Order
    {
        public Guid OrderID { get; set; }
        public Guid BuyerID { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatus OrderStatu { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
