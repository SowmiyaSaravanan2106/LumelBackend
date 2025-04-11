namespace AppStore.Model
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string? CustomerAddress { get; set; }

        public int? RegionId { get; set; }
        public Region? Region { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }


}
