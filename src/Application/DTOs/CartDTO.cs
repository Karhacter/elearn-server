namespace elearn_server.Application.DTOs
{

    public class CartDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<CartItemDTO>? Items { get; set; }
        public double Total { get; set; }
    }
}