using System.ComponentModel.DataAnnotations;
using elearn_server.Domain.Enums;

namespace elearn_server.Domain.Entities;

public class QuizQuestion : BaseEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int QuizId { get; set; }

    public Quiz? Quiz { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;

    public QuizQuestionType Type { get; set; } = QuizQuestionType.SingleChoice;

    [Range(1, 100)]
    public int Points { get; set; } = 1;

    [Range(1, 1000)]
    public int Order { get; set; } = 1;

    [MaxLength(1000)]
    public string? CorrectTextAnswer { get; set; }

    public ICollection<QuizAnswerOption>? Options { get; set; }
    public ICollection<QuizAttemptAnswer>? AttemptAnswers { get; set; }
}
