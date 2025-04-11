namespace AppStore.DTO
{
    public class ProductRevenueDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal TotalRevenue { get; set; }
    }
}
