namespace BackendAPI.DTO
{
    public class RegionRevenueDto
    {
        public int RegionId { get; set; }
        public string RegionName { get; set; } = string.Empty;
        public decimal TotalRevenue { get; set; }
    }
}
