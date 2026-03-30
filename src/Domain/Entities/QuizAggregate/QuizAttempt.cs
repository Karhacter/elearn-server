using System.ComponentModel.DataAnnotations;
using elearn_server.Domain.Enums;

namespace elearn_server.Domain.Entities;

public class QuizAttempt : BaseEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int QuizId { get; set; }

    public Quiz? Quiz { get; set; }

    [Required]
    public int UserId { get; set; }

    public User? User { get; set; }

    [Range(1, 1000)]
    public int AttemptNumber { get; set; } = 1;

    public QuizAttemptStatus Status { get; set; } = QuizAttemptStatus.InProgress;

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAt { get; set; }

    [Range(0, 100)]
    public double ScorePercent { get; set; }

    public bool IsPassed { get; set; }

    public ICollection<QuizAttemptAnswer>? Answers { get; set; }
}
