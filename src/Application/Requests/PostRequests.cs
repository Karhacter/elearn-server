using System.ComponentModel.DataAnnotations;

namespace elearn_server.Application.Requests;

public class CreatePostRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    [Required]
    public string Content { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    [Required]
    public int TopicId { get; set; }
}

public class UpdatePostRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    [Required]
    public string Content { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    [Required]
    public int TopicId { get; set; }
}
