namespace elearn_server.DTO
{
    public class CartItemDTO
    {
        public int CourseID { get; set; }
        public string CourseTitle { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public string Image { get; set; }
        public double Subtotal { get; set; }
    }
}