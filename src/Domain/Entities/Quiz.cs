using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace elearn_server.Domain.Entities;

public class Quiz : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "Course ID is required")]
    public int CourseId { get; set; }

    [ForeignKey("CourseId")]
    public Course? Course { get; set; }

    [Required(ErrorMessage = "Quiz title is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be 3-100 characters")]
    public string Title { get; set; } = string.Empty;

    [Range(0, 100, ErrorMessage = "Pass score must be between 0 and 100")]
    public double PassScore { get; set; } = 70;

    [Range(1, 720, ErrorMessage = "Time limit must be between 1 and 720 minutes")]
    public int? TimeLimitMinutes { get; set; }

    [Range(1, 20, ErrorMessage = "Max attempts must be between 1 and 20")]
    public int MaxAttempts { get; set; } = 3;

    public bool IsRandomOrderEnabled { get; set; }

    public ICollection<QuizQuestion>? Questions { get; set; }
    public ICollection<QuizAttempt>? Attempts { get; set; }
}
