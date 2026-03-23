namespace elearn_server.Application.Requests
{
    // Model để nhận dữ liệu từ client
    public class AddToCartRequest
    {
        public int UserId { get; set; }
        public int CourseID { get; set; }
        public int Quantity { get; set; } = 1;
    }

}