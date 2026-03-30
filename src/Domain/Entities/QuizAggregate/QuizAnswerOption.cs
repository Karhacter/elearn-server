using System.ComponentModel.DataAnnotations;

namespace elearn_server.Domain.Entities;

public class QuizAnswerOption : BaseEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int QuestionId { get; set; }

    public QuizQuestion? Question { get; set; }

    [Required]
    [MaxLength(500)]
    public string Content { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }

    [Range(1, 1000)]
    public int Order { get; set; } = 1;
}
