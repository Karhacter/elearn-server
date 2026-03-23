namespace elearn_server.Application.Requests
{
    public class RemoveCartItemRequest
    {
        public int UserId { get; set; }
        public int CourseID { get; set; }
    }
}