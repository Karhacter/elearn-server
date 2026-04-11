using System.ComponentModel.DataAnnotations;

namespace elearn_server.Domain.Entities;

public class Post : BaseEntity
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public string? ThumbnailUrl { get; set; }

    [Required]
    public int TopicId { get; set; }

    public bool IsDeleted { get; set; } = false;

    // Navigation property
    public Topic Topic { get; set; } = null!;
}
