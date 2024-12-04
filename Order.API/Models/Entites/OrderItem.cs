namespace Order.API.Models.Entites
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public long Count { get; set; }
        public decimal Price { get; set; }

        //relational
        public Order Order { get; set; }
    }
}
