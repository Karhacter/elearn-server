namespace elearn_server.Application.Responses;

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
