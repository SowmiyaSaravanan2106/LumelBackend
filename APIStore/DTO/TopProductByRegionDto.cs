namespace BackendAPI.DTO
{
        public class TopProductByRegionDto
        {
            public int RegionId { get; set; }
            public string RegionName { get; set; } = string.Empty;
            public int ProductId { get; set; }
            public string ProductName { get; set; } = string.Empty;
            public int TotalQuantitySold { get; set; }
        }

}
