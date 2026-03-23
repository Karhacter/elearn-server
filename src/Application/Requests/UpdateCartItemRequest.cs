namespace elearn_server.Application.Requests
{
    public class UpdateCartItemRequest
    {
        public int UserId { get; set; }
        public int CourseID { get; set; }
        public int Quantity { get; set; }

    }
}