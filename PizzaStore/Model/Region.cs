namespace AppStore.Model
{
    public class Region
    {
        public int RegionId { get; set; }
        public string RegionName { get; set; } = string.Empty;

        public ICollection<Customer> Customers { get; set; } = new List<Customer>();
    }

}
