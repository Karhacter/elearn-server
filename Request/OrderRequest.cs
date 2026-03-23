namespace elearn_server.Request
{
    public class OrderRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public int StatusOrderId { get; set; }
        public int UserId { get; set; }
        public List<OrderItemRequest> Items { get; set; }
    }
}