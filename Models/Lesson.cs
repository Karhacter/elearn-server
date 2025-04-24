using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace elearn_server.Models;

public class Lesson : BaseEntity
{
    [Key]
    public int LessonId { get; set; }

    [Required(ErrorMessage = "Lesson title is required")]
    [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Content URL is required")]
    [Url(ErrorMessage = "Invalid URL format")]
    public string ContentUrl { get; set; }

    [Range(1, 1000, ErrorMessage = "Duration must be between 1 and 1000 minutes")]
    public int Duration { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Order must be greater than 0")]
    public int Order { get; set; }

    [ForeignKey("Course")]
    [Required(ErrorMessage = "Course ID is required")]
    public int CourseId { get; set; }
    public Course Course { get; set; }
}
