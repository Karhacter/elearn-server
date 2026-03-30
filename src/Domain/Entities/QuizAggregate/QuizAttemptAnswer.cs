using System.ComponentModel.DataAnnotations;

namespace elearn_server.Domain.Entities;

public class QuizAttemptAnswer : BaseEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int AttemptId { get; set; }

    public QuizAttempt? Attempt { get; set; }

    [Required]
    public int QuestionId { get; set; }

    public QuizQuestion? Question { get; set; }

    [MaxLength(200)]
    public string? SelectedOptionIdsCsv { get; set; }

    [MaxLength(1000)]
    public string? TextAnswer { get; set; }

    public bool IsCorrect { get; set; }

    [Range(0, 100)]
    public double AwardedPoints { get; set; }
}
