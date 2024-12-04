namespace Order.API.ViewModels
{
    public class CreateOrderItemViewModel
    {
        public Guid ProductId { get; set; }
        public long Count { get; set; }
        public decimal Price { get; set; }
    }
}
