using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using elearn_server.Domain.Enums;

namespace elearn_server.Domain.Entities;

public class Lesson : BaseEntity
{
    [Key]
    public int LessonId { get; set; }

    [Required(ErrorMessage = "Lesson title is required")]
    [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
    public string Title { get; set; }

    [Url(ErrorMessage = "Invalid URL format")]
    public string? ContentUrl { get; set; }

    [Range(1, 1000, ErrorMessage = "Duration must be between 1 and 1000 minutes")]
    public int Duration { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Order must be greater than 0")]
    public int Order { get; set; }

    [Required]
    public LessonType Type { get; set; } = LessonType.Video;

    [ForeignKey(nameof(CourseSection))]
    [Required(ErrorMessage = "Section ID is required")]
    public int SectionId { get; set; }
    public CourseSection? CourseSection { get; set; }

    [ForeignKey(nameof(Course))]
    [Required(ErrorMessage = "Course ID is required")]
    public int CourseId { get; set; }
    public Course? Course { get; set; }
}
