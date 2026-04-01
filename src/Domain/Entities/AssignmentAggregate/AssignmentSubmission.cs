using System.ComponentModel.DataAnnotations;

namespace elearn_server.Domain.Entities;

public class AssignmentSubmission : BaseEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int AssignmentId { get; set; }
    public Assignment? Assignment { get; set; }

    [Required]
    public int StudentId { get; set; }
    public User? Student { get; set; }

    [MaxLength(2000)]
    public string? FileUrl { get; set; }

    [MaxLength(4000)]
    public string? TextSubmission { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public bool IsLate { get; set; }

    [Range(0, 100)]
    public double? Grade { get; set; }

    [MaxLength(2000)]
    public string? InstructorFeedback { get; set; }

    public int? GradedByInstructorId { get; set; }
    public User? GradedByInstructor { get; set; }
    public DateTime? GradedAt { get; set; }
}
