namespace BackendAPI.DTO
{
    public class CategoryRevenueDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal TotalRevenue { get; set; }
    }
}
