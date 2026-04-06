namespace elearn_server.Application.Requests;

using System.ComponentModel.DataAnnotations;
using elearn_server.Domain.Enums;

public class CourseCreateRequest
{
    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MinLength(20)]
    public string Description { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Discount { get; set; }

    [Required]
    public int GenreId { get; set; }

    [Range(10, 10000)]
    public int Duration { get; set; }

    [Url]
    public string? Thumbnail { get; set; }

    public string? Image { get; set; }

    [Required]
    public int InstructorId { get; set; }

    [MaxLength(200)]
    public string? Slug { get; set; }

    public bool IsSequential { get; set; }

    public CourseStatus Status { get; set; }

    public List<string> LearningOutcomes { get; set; } = new();
    public List<string> Requirements { get; set; } = new();
    public List<string> TargetAudiences { get; set; } = new();
}
