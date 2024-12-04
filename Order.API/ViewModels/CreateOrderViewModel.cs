namespace Order.API.ViewModels
{
    public class CreateOrderViewModel
    {
        public Guid BuyerID { get; set; }
        public List<CreateOrderItemViewModel> OrderItems { get; set; }

    }

}
