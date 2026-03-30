namespace elearn_server.Application.Responses;

public class AuthenticatedUserResponse
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
}

public class LoginResponse
{
    public AuthenticatedUserResponse User { get; set; } = new();
    public DateTime ExpiresAtUtc { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

public class AuthCheckResponse
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class CategoryResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class CourseResponse
{
    public int CourseId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Slug { get; set; }
    public string? Status { get; set; }
    public bool IsSequential { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    public int GenreId { get; set; }
    public string? GenreName { get; set; }
    public string? Image { get; set; }
    public string? Thumbnail { get; set; }
    public int Duration { get; set; }
    public int InstructorId { get; set; }
    public string? InstructorName { get; set; }
    public IReadOnlyCollection<string> LearningOutcomes { get; set; } = Array.Empty<string>();
    public IReadOnlyCollection<string> Requirements { get; set; } = Array.Empty<string>();
    public IReadOnlyCollection<string> TargetAudiences { get; set; } = Array.Empty<string>();
}

public class SectionResponse
{
    public int SectionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Order { get; set; }
    public IReadOnlyCollection<LessonResponse> Lessons { get; set; } = Array.Empty<LessonResponse>();
}

public class LessonResponse
{
    public int LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ContentUrl { get; set; }
    public string Type { get; set; } = string.Empty;
    public int Duration { get; set; }
    public int Order { get; set; }
    public int SectionId { get; set; }
}

public class CoursePreviewResponse
{
    public int CourseId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Slug { get; set; }
    public string? Status { get; set; }
    public IReadOnlyCollection<string> LearningOutcomes { get; set; } = Array.Empty<string>();
    public IReadOnlyCollection<string> Requirements { get; set; } = Array.Empty<string>();
    public IReadOnlyCollection<string> TargetAudiences { get; set; } = Array.Empty<string>();
    public IReadOnlyCollection<SectionResponse> Sections { get; set; } = Array.Empty<SectionResponse>();
}

public class LessonProgressResponse
{
    public int LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int SectionId { get; set; }
    public int Order { get; set; }
    public bool IsCompleted { get; set; }
    public int WatchPositionSeconds { get; set; }
    public DateTime? LastViewedAt { get; set; }
}

public class CourseProgressResponse
{
    public int CourseId { get; set; }
    public string? CourseTitle { get; set; }
    public string? Thumbnail { get; set; }
    public double ProgressPercent { get; set; }
    public int CompletedLessons { get; set; }
    public int TotalLessons { get; set; }
    public int? LastViewedLessonId { get; set; }
    public DateTime? LastViewedAt { get; set; }
    public IReadOnlyCollection<LessonProgressResponse> Lessons { get; set; } = Array.Empty<LessonProgressResponse>();
}

public class MyLearningItemResponse
{
    public int CourseId { get; set; }
    public string? CourseTitle { get; set; }
    public string? Thumbnail { get; set; }
    public double ProgressPercent { get; set; }
    public int? LastViewedLessonId { get; set; }
    public DateTime? LastViewedAt { get; set; }
}

public class QuizAnswerOptionResponse
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int Order { get; set; }
}

public class QuizQuestionResponse
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Points { get; set; }
    public int Order { get; set; }
    public string? CorrectTextAnswer { get; set; }
    public IReadOnlyCollection<QuizAnswerOptionResponse> Options { get; set; } = Array.Empty<QuizAnswerOptionResponse>();
}

public class QuizQuestionForAttemptResponse
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Points { get; set; }
    public int Order { get; set; }
    public IReadOnlyCollection<QuizAnswerOptionForAttemptResponse> Options { get; set; } = Array.Empty<QuizAnswerOptionForAttemptResponse>();
}

public class QuizAnswerOptionForAttemptResponse
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Order { get; set; }
}

public class QuizResponse
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public double PassScore { get; set; }
    public int? TimeLimitMinutes { get; set; }
    public int MaxAttempts { get; set; }
    public bool IsRandomOrderEnabled { get; set; }
    public IReadOnlyCollection<QuizQuestionResponse> Questions { get; set; } = Array.Empty<QuizQuestionResponse>();
}

public class QuizAttemptStartResponse
{
    public int AttemptId { get; set; }
    public int QuizId { get; set; }
    public int AttemptNumber { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public IReadOnlyCollection<QuizQuestionForAttemptResponse> Questions { get; set; } = Array.Empty<QuizQuestionForAttemptResponse>();
}

public class QuizAttemptAnswerResponse
{
    public int AttemptId { get; set; }
    public int QuestionId { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class QuizAttemptResultResponse
{
    public int AttemptId { get; set; }
    public int QuizId { get; set; }
    public int AttemptNumber { get; set; }
    public string Status { get; set; } = string.Empty;
    public double ScorePercent { get; set; }
    public bool IsPassed { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
}

public class QuizResultSummaryResponse
{
    public int QuizId { get; set; }
    public bool HasPassed { get; set; }
    public double BestScorePercent { get; set; }
    public int TotalAttempts { get; set; }
    public QuizAttemptResultResponse? LatestAttempt { get; set; }
}

public class OrderDetailResponse
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int CourseId { get; set; }
    public string? CourseTitle { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Amount { get; set; }
}

public class OrderResponse
{
    public int OrderId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string Status { get; set; } = string.Empty;
    public IReadOnlyCollection<OrderDetailResponse> Items { get; set; } = Array.Empty<OrderDetailResponse>();
}

public class WishlistResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public string? CourseTitle { get; set; }
    public DateTime AddedDate { get; set; }
}

public class PaymentResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public string? CourseTitle { get; set; }
    public double Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
}

public class EnrollmentResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public string? CourseTitle { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public bool IsActive { get; set; }
}

public class CommentResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CommentDate { get; set; }
}

public class CertificateResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public string CertificateUrl { get; set; } = string.Empty;
    public DateTime DateIssued { get; set; }
}

public class ImageUploadResponse
{
    public string ImageUrl { get; set; } = string.Empty;
}
