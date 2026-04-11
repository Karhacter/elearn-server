namespace elearn_server.Application.Responses;

public class PostResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public int TopicId { get; set; }
    public string? TopicName { get; set; }
    public bool IsDeleted { get; set; }
}
