namespace elearn_server.Application.Requests
{
    public class OrderDetailRequest
    {
        public int OrderId { get; set; }
        public int CourseID { get; set; }
        public int Quantity { get; set; }
    }
}