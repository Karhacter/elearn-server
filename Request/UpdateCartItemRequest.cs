namespace elearn_server.Request
{
    public class UpdateCartItemRequest
    {
        public int UserId { get; set; }
        public int CourseID { get; set; }
        public int Quantity { get; set; }

    }
}