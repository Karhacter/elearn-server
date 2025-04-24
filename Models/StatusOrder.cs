namespace elearn_server.Models
{
    public class StatusOrder
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Order>? Orders { get; set; } = new List<Order>();
    }
}