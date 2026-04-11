using System.ComponentModel.DataAnnotations;

namespace elearn_server.Application.Requests;

public class CreateTopicRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateTopicRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
