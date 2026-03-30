using elearn_server.Application.Common;
using elearn_server.Application.Interfaces;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;
using elearn_server.Domain.Entities;
using elearn_server.Domain.Enums;
using elearn_server.Domain.Interfaces;

namespace elearn_server.Infrastructure.Services.Core.Quizzes;

public class QuizService(IQuizRepository repository) : IQuizService
{
    public async Task<ServiceResult<QuizResponse>> CreateQuizAsync(QuizUpsertRequest request)
    {
        if (!await repository.CourseExistsAsync(request.CourseId))
        {
            return ServiceResult<QuizResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.");
        }

        if (!IsValidQuizConfig(request, out var quizError))
        {
            return ServiceResult<QuizResponse>.Fail(StatusCodes.Status400BadRequest, quizError);
        }

        var quiz = new Quiz
        {
            CourseId = request.CourseId,
            Title = request.Title.Trim(),
            PassScore = request.PassScore,
            TimeLimitMinutes = request.TimeLimitMinutes is > 0 ? request.TimeLimitMinutes : null,
            MaxAttempts = request.MaxAttempts,
            IsRandomOrderEnabled = request.IsRandomOrderEnabled,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await repository.AddQuizAsync(quiz);
        await repository.SaveChangesAsync();
        var created = await repository.GetQuizByIdWithQuestionsAsync(quiz.Id);
        return ServiceResult<QuizResponse>.Created(ToQuizResponse(created!), "Quiz created successfully.");
    }

    public async Task<ServiceResult<QuizResponse>> UpdateQuizAsync(int quizId, QuizUpsertRequest request)
    {
        var quiz = await repository.GetQuizByIdAsync(quizId);
        if (quiz is null)
        {
            return ServiceResult<QuizResponse>.Fail(StatusCodes.Status404NotFound, "Quiz not found.");
        }

        if (!await repository.CourseExistsAsync(request.CourseId))
        {
            return ServiceResult<QuizResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.");
        }

        if (!IsValidQuizConfig(request, out var quizError))
        {
            return ServiceResult<QuizResponse>.Fail(StatusCodes.Status400BadRequest, quizError);
        }

        quiz.CourseId = request.CourseId;
        quiz.Title = request.Title.Trim();
        quiz.PassScore = request.PassScore;
        quiz.TimeLimitMinutes = request.TimeLimitMinutes is > 0 ? request.TimeLimitMinutes : null;
        quiz.MaxAttempts = request.MaxAttempts;
        quiz.IsRandomOrderEnabled = request.IsRandomOrderEnabled;
        quiz.UpdatedAt = DateTime.UtcNow;

        await repository.SaveChangesAsync();
        var updated = await repository.GetQuizByIdWithQuestionsAsync(quizId);
        return ServiceResult<QuizResponse>.Ok(ToQuizResponse(updated!), "Quiz updated successfully.");
    }

    public async Task<ServiceResult<object>> DeleteQuizAsync(int quizId)
    {
        var quiz = await repository.GetQuizByIdAsync(quizId);
        if (quiz is null)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status404NotFound, "Quiz not found.");
        }

        repository.RemoveQuiz(quiz);
        await repository.SaveChangesAsync();
        return ServiceResult<object>.Ok(null, "Quiz deleted successfully.");
    }

    public async Task<ServiceResult<QuizResponse>> CreateQuestionAsync(int quizId, QuizQuestionUpsertRequest request)
    {
        var quiz = await repository.GetQuizByIdAsync(quizId);
        if (quiz is null)
        {
            return ServiceResult<QuizResponse>.Fail(StatusCodes.Status404NotFound, "Quiz not found.");
        }

        if (!ValidateQuestionRequest(request, out var validationError))
        {
            return ServiceResult<QuizResponse>.Fail(StatusCodes.Status400BadRequest, validationError);
        }

        var question = new QuizQuestion
        {
            QuizId = quizId,
            Content = request.Content.Trim(),
            Type = request.Type,
            Points = request.Points,
            Order = request.Order,
            CorrectTextAnswer = request.Type == QuizQuestionType.Text ? request.CorrectTextAnswer?.Trim() : null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Options = request.Type == QuizQuestionType.Text
                ? new List<QuizAnswerOption>()
                : request.Options.Select(o => new QuizAnswerOption
                {
                    Content = o.Content.Trim(),
                    IsCorrect = o.IsCorrect,
                    Order = o.Order,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }).ToList()
        };

        await repository.AddQuestionAsync(question);
        await repository.SaveChangesAsync();
        var updatedQuiz = await repository.GetQuizByIdWithQuestionsAsync(quizId);
        return ServiceResult<QuizResponse>.Ok(ToQuizResponse(updatedQuiz!), "Question created successfully.");
    }

    public async Task<ServiceResult<QuizResponse>> UpdateQuestionAsync(int quizId, int questionId, QuizQuestionUpsertRequest request)
    {
        var question = await repository.GetQuestionByIdWithOptionsAsync(questionId);
        if (question is null || question.QuizId != quizId)
        {
            return ServiceResult<QuizResponse>.Fail(StatusCodes.Status404NotFound, "Question not found.");
        }

        if (!ValidateQuestionRequest(request, out var validationError))
        {
            return ServiceResult<QuizResponse>.Fail(StatusCodes.Status400BadRequest, validationError);
        }

        question.Content = request.Content.Trim();
        question.Type = request.Type;
        question.Points = request.Points;
        question.Order = request.Order;
        question.CorrectTextAnswer = request.Type == QuizQuestionType.Text ? request.CorrectTextAnswer?.Trim() : null;
        question.UpdatedAt = DateTime.UtcNow;

        if (request.Type == QuizQuestionType.Text)
        {
            question.Options?.Clear();
        }
        else
        {
            question.Options ??= new List<QuizAnswerOption>();
            question.Options.Clear();
            foreach (var option in request.Options)
            {
                question.Options.Add(new QuizAnswerOption
                {
                    Content = option.Content.Trim(),
                    IsCorrect = option.IsCorrect,
                    Order = option.Order,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }

        await repository.SaveChangesAsync();
        var updatedQuiz = await repository.GetQuizByIdWithQuestionsAsync(quizId);
        return ServiceResult<QuizResponse>.Ok(ToQuizResponse(updatedQuiz!), "Question updated successfully.");
    }

    public async Task<ServiceResult<object>> DeleteQuestionAsync(int quizId, int questionId)
    {
        var question = await repository.GetQuestionByIdAsync(questionId);
        if (question is null || question.QuizId != quizId)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status404NotFound, "Question not found.");
        }

        repository.RemoveQuestion(question);
        await repository.SaveChangesAsync();
        return ServiceResult<object>.Ok(null, "Question deleted successfully.");
    }

    public async Task<ServiceResult<QuizAttemptStartResponse>> StartAttemptAsync(int quizId, int userId)
    {
        var quiz = await repository.GetQuizByIdWithQuestionsAsync(quizId);
        if (quiz is null)
        {
            return ServiceResult<QuizAttemptStartResponse>.Fail(StatusCodes.Status404NotFound, "Quiz not found.");
        }

        if (!await repository.UserExistsAsync(userId))
        {
            return ServiceResult<QuizAttemptStartResponse>.Fail(StatusCodes.Status404NotFound, "User not found.");
        }

        if (!await repository.IsUserEnrolledAsync(userId, quiz.CourseId))
        {
            return ServiceResult<QuizAttemptStartResponse>.Fail(StatusCodes.Status403Forbidden, "You are not enrolled in this course.");
        }

        var existingAttempts = await repository.GetAttemptsByQuizAndUserAsync(quizId, userId);
        var inProgressAttempt = existingAttempts.FirstOrDefault(a => a.Status == QuizAttemptStatus.InProgress);
        if (inProgressAttempt is not null)
        {
            var inProgressWithDetails = await repository.GetAttemptByIdWithDetailsAsync(inProgressAttempt.Id);
            if (inProgressWithDetails is not null)
            {
                var autoSubmitted = await TryAutoSubmitIfTimedOutAsync(inProgressWithDetails);
                if (!autoSubmitted)
                {
                    return ServiceResult<QuizAttemptStartResponse>.Fail(StatusCodes.Status400BadRequest, "You already have an attempt in progress.");
                }

                existingAttempts = await repository.GetAttemptsByQuizAndUserAsync(quizId, userId);
            }
        }

        if (existingAttempts.Count >= quiz.MaxAttempts)
        {
            return ServiceResult<QuizAttemptStartResponse>.Fail(StatusCodes.Status400BadRequest, "Maximum attempts reached.");
        }

        var questions = quiz.Questions?.OrderBy(q => q.Order).ToList() ?? new List<QuizQuestion>();
        if (questions.Count == 0)
        {
            return ServiceResult<QuizAttemptStartResponse>.Fail(StatusCodes.Status400BadRequest, "Quiz has no questions.");
        }

        if (quiz.IsRandomOrderEnabled)
        {
            questions = questions.OrderBy(_ => Random.Shared.Next()).ToList();
        }

        var attempt = new QuizAttempt
        {
            QuizId = quizId,
            UserId = userId,
            AttemptNumber = existingAttempts.Count + 1,
            Status = QuizAttemptStatus.InProgress,
            StartedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await repository.AddAttemptAsync(attempt);
        await repository.SaveChangesAsync();

        return ServiceResult<QuizAttemptStartResponse>.Created(new QuizAttemptStartResponse
        {
            AttemptId = attempt.Id,
            QuizId = quizId,
            AttemptNumber = attempt.AttemptNumber,
            StartedAt = attempt.StartedAt,
            ExpiresAt = quiz.TimeLimitMinutes.HasValue ? attempt.StartedAt.AddMinutes(quiz.TimeLimitMinutes.Value) : null,
            Questions = questions.Select(ToAttemptQuestionResponse).ToList()
        }, "Quiz attempt started.");
    }

    public async Task<ServiceResult<QuizAttemptAnswerResponse>> SaveAnswerAsync(int attemptId, int userId, QuizAttemptAnswerRequest request)
    {
        var attempt = await repository.GetAttemptByIdWithDetailsAsync(attemptId);
        if (attempt is null || attempt.UserId != userId)
        {
            return ServiceResult<QuizAttemptAnswerResponse>.Fail(StatusCodes.Status404NotFound, "Attempt not found.");
        }

        if (attempt.Status != QuizAttemptStatus.InProgress)
        {
            return ServiceResult<QuizAttemptAnswerResponse>.Fail(StatusCodes.Status400BadRequest, "Attempt is not in progress.");
        }

        if (await TryAutoSubmitIfTimedOutAsync(attempt))
        {
            return ServiceResult<QuizAttemptAnswerResponse>.Fail(StatusCodes.Status400BadRequest, "Attempt timed out and was auto-submitted.");
        }

        var question = attempt.Quiz?.Questions?.SingleOrDefault(q => q.Id == request.QuestionId);
        if (question is null)
        {
            return ServiceResult<QuizAttemptAnswerResponse>.Fail(StatusCodes.Status404NotFound, "Question not found in this quiz.");
        }

        if (!ValidateAnswerPayload(question, request, out var answerError))
        {
            return ServiceResult<QuizAttemptAnswerResponse>.Fail(StatusCodes.Status400BadRequest, answerError);
        }

        var existingAnswer = await repository.GetAttemptAnswerAsync(attemptId, request.QuestionId);
        if (existingAnswer is null)
        {
            existingAnswer = new QuizAttemptAnswer
            {
                AttemptId = attemptId,
                QuestionId = request.QuestionId,
                SelectedOptionIdsCsv = ToCsv(request.SelectedOptionIds),
                TextAnswer = request.TextAnswer?.Trim(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await repository.AddAttemptAnswerAsync(existingAnswer);
        }
        else
        {
            existingAnswer.SelectedOptionIdsCsv = ToCsv(request.SelectedOptionIds);
            existingAnswer.TextAnswer = request.TextAnswer?.Trim();
            existingAnswer.UpdatedAt = DateTime.UtcNow;
        }

        await repository.SaveChangesAsync();
        return ServiceResult<QuizAttemptAnswerResponse>.Ok(new QuizAttemptAnswerResponse
        {
            AttemptId = attemptId,
            QuestionId = request.QuestionId,
            Message = "Answer saved."
        });
    }

    public async Task<ServiceResult<QuizAttemptResultResponse>> SubmitAttemptAsync(int attemptId, int userId)
    {
        var attempt = await repository.GetAttemptByIdWithDetailsAsync(attemptId);
        if (attempt is null || attempt.UserId != userId)
        {
            return ServiceResult<QuizAttemptResultResponse>.Fail(StatusCodes.Status404NotFound, "Attempt not found.");
        }

        if (attempt.Status is QuizAttemptStatus.Submitted or QuizAttemptStatus.TimedOut)
        {
            return ServiceResult<QuizAttemptResultResponse>.Ok(ToAttemptResultResponse(attempt), "Attempt already finalized.");
        }

        var autoSubmitted = await TryAutoSubmitIfTimedOutAsync(attempt);
        if (autoSubmitted)
        {
            var refreshed = await repository.GetAttemptByIdAsync(attemptId);
            return ServiceResult<QuizAttemptResultResponse>.Ok(ToAttemptResultResponse(refreshed!), "Attempt timed out and was auto-submitted.");
        }

        EvaluateAttempt(attempt, QuizAttemptStatus.Submitted);
        await repository.SaveChangesAsync();
        return ServiceResult<QuizAttemptResultResponse>.Ok(ToAttemptResultResponse(attempt), "Attempt submitted.");
    }

    public async Task<ServiceResult<IReadOnlyCollection<QuizAttemptResultResponse>>> GetMyAttemptHistoryAsync(int quizId, int userId)
    {
        var attempts = await repository.GetAttemptsByQuizAndUserAsync(quizId, userId);
        await SyncExpiredAttemptsAsync(attempts);
        attempts = await repository.GetAttemptsByQuizAndUserAsync(quizId, userId);
        if (attempts.Count == 0)
        {
            return ServiceResult<IReadOnlyCollection<QuizAttemptResultResponse>>.Fail(StatusCodes.Status404NotFound, "No attempts found.");
        }

        return ServiceResult<IReadOnlyCollection<QuizAttemptResultResponse>>.Ok(attempts.Select(ToAttemptResultResponse).ToList());
    }

    public async Task<ServiceResult<QuizResultSummaryResponse>> GetMyResultSummaryAsync(int quizId, int userId)
    {
        var attempts = await repository.GetAttemptsByQuizAndUserAsync(quizId, userId);
        await SyncExpiredAttemptsAsync(attempts);
        attempts = await repository.GetAttemptsByQuizAndUserAsync(quizId, userId);
        if (attempts.Count == 0)
        {
            return ServiceResult<QuizResultSummaryResponse>.Fail(StatusCodes.Status404NotFound, "No attempts found.");
        }

        var latest = attempts.OrderByDescending(a => a.AttemptNumber).First();
        var bestScore = attempts.Max(a => a.ScorePercent);

        return ServiceResult<QuizResultSummaryResponse>.Ok(new QuizResultSummaryResponse
        {
            QuizId = quizId,
            HasPassed = attempts.Any(a => a.IsPassed),
            BestScorePercent = bestScore,
            TotalAttempts = attempts.Count,
            LatestAttempt = ToAttemptResultResponse(latest)
        });
    }

    private static bool IsValidQuizConfig(QuizUpsertRequest request, out string error)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            error = "Quiz title is required.";
            return false;
        }

        if (request.PassScore < 0 || request.PassScore > 100)
        {
            error = "Pass score must be from 0 to 100.";
            return false;
        }

        if (request.MaxAttempts < 1)
        {
            error = "Max attempts must be at least 1.";
            return false;
        }

        if (request.TimeLimitMinutes.HasValue && request.TimeLimitMinutes.Value < 1)
        {
            error = "Time limit must be at least 1 minute.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    private static bool ValidateQuestionRequest(QuizQuestionUpsertRequest request, out string error)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            error = "Question content is required.";
            return false;
        }

        if (request.Points <= 0)
        {
            error = "Question points must be greater than 0.";
            return false;
        }

        if (request.Type == QuizQuestionType.Text)
        {
            if (string.IsNullOrWhiteSpace(request.CorrectTextAnswer))
            {
                error = "Correct text answer is required for text question.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        if (request.Options.Count < 2)
        {
            error = "Choice questions must have at least 2 options.";
            return false;
        }

        var correctCount = request.Options.Count(o => o.IsCorrect);
        if (correctCount == 0)
        {
            error = "At least one correct option is required.";
            return false;
        }

        if (request.Type == QuizQuestionType.SingleChoice && correctCount != 1)
        {
            error = "Single choice question must have exactly one correct option.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    private static bool ValidateAnswerPayload(QuizQuestion question, QuizAttemptAnswerRequest request, out string error)
    {
        if (question.Type == QuizQuestionType.Text)
        {
            if (string.IsNullOrWhiteSpace(request.TextAnswer))
            {
                error = "Text answer is required.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        if (request.SelectedOptionIds.Count == 0)
        {
            error = "At least one option must be selected.";
            return false;
        }

        var validOptionIds = question.Options?.Select(o => o.Id).ToHashSet() ?? new HashSet<int>();
        if (request.SelectedOptionIds.Any(id => !validOptionIds.Contains(id)))
        {
            error = "Selected option does not belong to this question.";
            return false;
        }

        if (question.Type == QuizQuestionType.SingleChoice && request.SelectedOptionIds.Distinct().Count() != 1)
        {
            error = "Single choice question requires exactly one selected option.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    private async Task<bool> TryAutoSubmitIfTimedOutAsync(QuizAttempt attempt)
    {
        if (attempt.Quiz?.TimeLimitMinutes is null || attempt.Quiz.TimeLimitMinutes <= 0)
        {
            return false;
        }

        var expiresAt = attempt.StartedAt.AddMinutes(attempt.Quiz.TimeLimitMinutes.Value);
        if (DateTime.UtcNow <= expiresAt)
        {
            return false;
        }

        EvaluateAttempt(attempt, QuizAttemptStatus.TimedOut);
        await repository.SaveChangesAsync();
        return true;
    }

    private async Task SyncExpiredAttemptsAsync(IEnumerable<QuizAttempt> attempts)
    {
        foreach (var attempt in attempts.Where(a => a.Status == QuizAttemptStatus.InProgress))
        {
            var detail = await repository.GetAttemptByIdWithDetailsAsync(attempt.Id);
            if (detail is null)
            {
                continue;
            }

            await TryAutoSubmitIfTimedOutAsync(detail);
        }
    }

    private static void EvaluateAttempt(QuizAttempt attempt, QuizAttemptStatus finalStatus)
    {
        var questions = attempt.Quiz?.Questions?.ToList() ?? new List<QuizQuestion>();
        var answers = attempt.Answers?.ToList() ?? new List<QuizAttemptAnswer>();
        var totalPoints = questions.Sum(q => q.Points);
        var awardedPoints = 0d;

        foreach (var question in questions)
        {
            var answer = answers.SingleOrDefault(a => a.QuestionId == question.Id);
            var isCorrect = IsAnswerCorrect(question, answer);
            var questionPoints = isCorrect ? question.Points : 0;
            awardedPoints += questionPoints;

            if (answer is not null)
            {
                answer.IsCorrect = isCorrect;
                answer.AwardedPoints = questionPoints;
                answer.UpdatedAt = DateTime.UtcNow;
            }
        }

        attempt.ScorePercent = totalPoints == 0 ? 0 : Math.Round(awardedPoints * 100d / totalPoints, 2);
        attempt.IsPassed = attempt.ScorePercent >= (attempt.Quiz?.PassScore ?? 0);
        attempt.EndedAt = DateTime.UtcNow;
        attempt.Status = finalStatus;
        attempt.UpdatedAt = DateTime.UtcNow;
    }

    private static bool IsAnswerCorrect(QuizQuestion question, QuizAttemptAnswer? answer)
    {
        if (answer is null)
        {
            return false;
        }

        if (question.Type == QuizQuestionType.Text)
        {
            return Normalize(question.CorrectTextAnswer) == Normalize(answer.TextAnswer);
        }

        var selected = ParseCsv(answer.SelectedOptionIdsCsv).ToHashSet();
        var correct = question.Options?.Where(o => o.IsCorrect).Select(o => o.Id).ToHashSet() ?? new HashSet<int>();
        return selected.SetEquals(correct);
    }

    private static string Normalize(string? value) => (value ?? string.Empty).Trim().ToLowerInvariant();

    private static string? ToCsv(IEnumerable<int> values)
    {
        var normalized = values.Distinct().OrderBy(v => v).ToList();
        return normalized.Count == 0 ? null : string.Join(",", normalized);
    }

    private static IEnumerable<int> ParseCsv(string? csv)
    {
        if (string.IsNullOrWhiteSpace(csv))
        {
            return Array.Empty<int>();
        }

        var items = new List<int>();
        foreach (var token in csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (int.TryParse(token, out var id))
            {
                items.Add(id);
            }
        }

        return items;
    }

    private static QuizResponse ToQuizResponse(Quiz quiz) => new()
    {
        Id = quiz.Id,
        CourseId = quiz.CourseId,
        Title = quiz.Title,
        PassScore = quiz.PassScore,
        TimeLimitMinutes = quiz.TimeLimitMinutes,
        MaxAttempts = quiz.MaxAttempts,
        IsRandomOrderEnabled = quiz.IsRandomOrderEnabled,
        Questions = quiz.Questions?.OrderBy(q => q.Order).Select(ToQuestionResponse).ToList() ?? new List<QuizQuestionResponse>()
    };

    private static QuizQuestionResponse ToQuestionResponse(QuizQuestion question) => new()
    {
        Id = question.Id,
        Content = question.Content,
        Type = question.Type.ToString(),
        Points = question.Points,
        Order = question.Order,
        CorrectTextAnswer = question.CorrectTextAnswer,
        Options = question.Options?.OrderBy(o => o.Order).Select(o => new QuizAnswerOptionResponse
        {
            Id = o.Id,
            Content = o.Content,
            IsCorrect = o.IsCorrect,
            Order = o.Order
        }).ToList() ?? new List<QuizAnswerOptionResponse>()
    };

    private static QuizQuestionForAttemptResponse ToAttemptQuestionResponse(QuizQuestion question) => new()
    {
        Id = question.Id,
        Content = question.Content,
        Type = question.Type.ToString(),
        Points = question.Points,
        Order = question.Order,
        Options = question.Options?.OrderBy(o => o.Order).Select(o => new QuizAnswerOptionForAttemptResponse
        {
            Id = o.Id,
            Content = o.Content,
            Order = o.Order
        }).ToList() ?? new List<QuizAnswerOptionForAttemptResponse>()
    };

    private static QuizAttemptResultResponse ToAttemptResultResponse(QuizAttempt attempt) => new()
    {
        AttemptId = attempt.Id,
        QuizId = attempt.QuizId,
        AttemptNumber = attempt.AttemptNumber,
        Status = attempt.Status.ToString(),
        ScorePercent = attempt.ScorePercent,
        IsPassed = attempt.IsPassed,
        StartedAt = attempt.StartedAt,
        EndedAt = attempt.EndedAt
    };
}
