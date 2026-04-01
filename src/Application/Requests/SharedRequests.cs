using System.ComponentModel.DataAnnotations;
using elearn_server.Domain.Enums;

namespace elearn_server.Application.Requests;

public class CategoryUpsertRequest
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }
}

public class CourseUpsertRequest
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

    public List<string> LearningOutcomes { get; set; } = new();
    public List<string> Requirements { get; set; } = new();
    public List<string> TargetAudiences { get; set; } = new();
}

public class UserUpdateRequest
{
    [Required]
    [MinLength(2)]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(Admin|Instructor|Student)$")]
    public string Role { get; set; } = string.Empty;

    [Url]
    public string? ProfilePicture { get; set; }
}

public class UpdateImageRequest
{
    [Required]
    public string ImageUrl { get; set; } = string.Empty;
}

public class LessonResumePositionRequest
{
    [Range(0, int.MaxValue)]
    public int WatchPositionSeconds { get; set; }
}

public class QuizUpsertRequest
{
    [Required]
    public int CourseId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Range(0, 100)]
    public double PassScore { get; set; } = 70;

    [Range(1, 720)]
    public int? TimeLimitMinutes { get; set; }

    [Range(1, 20)]
    public int MaxAttempts { get; set; } = 3;

    public bool IsRandomOrderEnabled { get; set; }
}

public class QuizAnswerOptionUpsertRequest
{
    [Required]
    [MaxLength(500)]
    public string Content { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }

    [Range(1, 1000)]
    public int Order { get; set; } = 1;
}

public class QuizQuestionUpsertRequest
{
    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;

    [Required]
    public QuizQuestionType Type { get; set; } = QuizQuestionType.SingleChoice;

    [Range(1, 100)]
    public int Points { get; set; } = 1;

    [Range(1, 1000)]
    public int Order { get; set; } = 1;

    [MaxLength(1000)]
    public string? CorrectTextAnswer { get; set; }

    public List<QuizAnswerOptionUpsertRequest> Options { get; set; } = new();
}

public class QuizAttemptAnswerRequest
{
    [Required]
    public int QuestionId { get; set; }

    public List<int> SelectedOptionIds { get; set; } = new();

    [MaxLength(1000)]
    public string? TextAnswer { get; set; }
}

public class AssignmentUpsertRequest
{
    [Required]
    public int CourseId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime DueDate { get; set; }
}

public class AssignmentSubmissionRequest
{
    [MaxLength(4000)]
    public string? TextSubmission { get; set; }
}

public class AssignmentGradeRequest
{
    [Range(0, 100)]
    public double Grade { get; set; }

    [MaxLength(2000)]
    public string? InstructorFeedback { get; set; }
}

public class WishlistCreateRequest
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int CourseId { get; set; }
}

public class PaymentCreateRequest
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int CourseId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public double Amount { get; set; }

    [Required]
    [MaxLength(20)]
    public string Method { get; set; } = string.Empty;
}

public class EnrollmentCreateRequest
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int CourseId { get; set; }
}

public class CommentCreateRequest
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int CourseId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Content { get; set; } = string.Empty;
}

public class ReviewUpsertRequest
{
    [Range(1, 5)]
    public int Score { get; set; }

    [MaxLength(500)]
    public string? Comment { get; set; }
}

public class ReviewReplyRequest
{
    [Required]
    [MaxLength(1000)]
    public string ReplyContent { get; set; } = string.Empty;
}

public class ReviewModerationRequest
{
    [Required]
    public ReviewStatus Status { get; set; }
}

public class CertificateCreateRequest
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int CourseId { get; set; }

    [Required]
    [Url]
    public string CertificateUrl { get; set; } = string.Empty;
}

public class CertificateGenerateRequest
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int CourseId { get; set; }
}

public class OrderDetailUpsertRequest
{
    [Required]
    public int OrderId { get; set; }

    [Required]
    public int CourseId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}

public class SectionCreateRequest
{
    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public int? Order { get; set; }
}

public class SectionUpdateRequest
{
    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public int? Order { get; set; }
}

public class LessonCreateRequest
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Url]
    public string? ContentUrl { get; set; }

    [Range(1, 1000)]
    public int Duration { get; set; }

    [Required]
    public string Type { get; set; } = "Video";

    public int? Order { get; set; }
}

public class LessonUpdateRequest
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Url]
    public string? ContentUrl { get; set; }

    [Range(1, 1000)]
    public int Duration { get; set; }

    [Required]
    public string Type { get; set; } = "Video";

    public int? Order { get; set; }
}

public class ReorderItemRequest
{
    [Required]
    public int Id { get; set; }

    [Range(1, int.MaxValue)]
    public int Order { get; set; }
}

public class SectionReorderRequest
{
    [Required]
    public List<ReorderItemRequest> Sections { get; set; } = new();
}

public class LessonReorderRequest
{
    [Required]
    public List<ReorderItemRequest> Lessons { get; set; } = new();
}
