using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace elearn_server.Domain.Entities;

public class LearningOutcome
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(500)]
    public string Content { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Order { get; set; }

    [Required]
    public int CourseId { get; set; }

    [ForeignKey(nameof(CourseId))]
    public Course? Course { get; set; }
}
