namespace AppStore.Model
{
    public class Order
    {
        public int OrderId { get; set; }

        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public DateTime OrderDate { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Discount { get; set; }
        public decimal ShippingCost { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }



}
