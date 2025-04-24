namespace elearn_server.Request
{
    public class OrderItemRequest
    {
        public int CourseID { get; set; } // ID của sản phẩm trong đơn hàng
        public int Quantity { get; set; } // Số lượng sản phẩm yêu cầu
    }
}