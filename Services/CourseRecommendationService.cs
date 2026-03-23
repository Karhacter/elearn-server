using System.Text;
using System.Text.Json;
using elearn_server.Data;
using elearn_server.DTO;
using elearn_server.Models;
using elearn_server.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace elearn_server.Services;

public class CourseRecommendationService : ICourseRecommendationService
{
    private readonly AppDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly OllamaOptions _ollamaOptions;
    private readonly ILogger<CourseRecommendationService> _logger;

    public CourseRecommendationService(
        AppDbContext context,
        IHttpClientFactory httpClientFactory,
        IOptions<OllamaOptions> ollamaOptions,
        ILogger<CourseRecommendationService> logger)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _ollamaOptions = ollamaOptions.Value;
        _logger = logger;
    }

    public async Task<CourseRecommendationResponseDTO> RecommendCoursesAsync(
        CourseRecommendationRequest request,
        CancellationToken cancellationToken = default)
    {
        var candidates = await LoadCandidateCoursesAsync(request, cancellationToken);
        if (candidates.Count == 0)
        {
            return new CourseRecommendationResponseDTO
            {
                RoadmapName = request.RoadmapName,
                Goal = request.Goal,
                UsedAi = false,
                GeneratedAt = DateTime.UtcNow,
                Message = "Khong tim thay khoa hoc phu hop voi lo trinh da chon."
            };
        }

        var heuristicRecommendations = BuildHeuristicRecommendations(candidates, request);
        if (!CanUseAi())
        {
            return new CourseRecommendationResponseDTO
            {
                RoadmapName = request.RoadmapName,
                Goal = request.Goal,
                UsedAi = false,
                GeneratedAt = DateTime.UtcNow,
                Message = "Dang dung che do fallback. Hay bat Ollama local va cau hinh muc Ollama trong appsettings de nhan goi y tu AI.",
                Recommendations = heuristicRecommendations
            };
        }

        try
        {
            var aiRecommendations = await GenerateAiRecommendationsAsync(
                request,
                candidates,
                heuristicRecommendations,
                cancellationToken);

            if (aiRecommendations.Count > 0)
            {
                return new CourseRecommendationResponseDTO
                {
                    RoadmapName = request.RoadmapName,
                    Goal = request.Goal,
                    UsedAi = true,
                    GeneratedAt = DateTime.UtcNow,
                    Message = "Da tao thu tu hoc bang AI dua tren lo trinh va danh sach khoa hoc hien co.",
                    Recommendations = aiRecommendations
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "AI recommendation failed. Falling back to heuristic ranking.");

            var failureMessage = BuildFailureMessage(ex);

            return new CourseRecommendationResponseDTO
            {
                RoadmapName = request.RoadmapName,
                Goal = request.Goal,
                UsedAi = false,
                GeneratedAt = DateTime.UtcNow,
                Message = failureMessage,
                Recommendations = heuristicRecommendations
            };
        }

        return new CourseRecommendationResponseDTO
        {
            RoadmapName = request.RoadmapName,
            Goal = request.Goal,
            UsedAi = false,
            GeneratedAt = DateTime.UtcNow,
            Message = "AI tam thoi khong kha dung, he thong da chuyen sang xep hang theo quy tac noi bo.",
            Recommendations = heuristicRecommendations
        };
    }

    private async Task<List<Course>> LoadCandidateCoursesAsync(
        CourseRecommendationRequest request,
        CancellationToken cancellationToken)
    {
        var query = _context.Courses
            .AsNoTracking()
            .Include(c => c.Genre)
            .AsQueryable();

        if (request.CategoryId.HasValue)
        {
            query = query.Where(c => c.GenreId == request.CategoryId.Value);
        }

        var courses = await query.ToListAsync(cancellationToken);
        if (courses.Count == 0)
        {
            return courses;
        }

        return courses
            .Select(course => new
            {
                Course = course,
                Score = CalculateHeuristicScore(course, request)
            })
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.Course.Duration)
            .Take(Math.Max(request.Limit * 3, 10))
            .Select(x => x.Course)
            .ToList();
    }

    private List<CourseRecommendationDTO> BuildHeuristicRecommendations(
        List<Course> candidates,
        CourseRecommendationRequest request)
    {
        return candidates
            .Select(course => new
            {
                Course = course,
                Score = CalculateHeuristicScore(course, request)
            })
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.Course.Duration)
            .Take(request.Limit)
            .Select((x, index) => new CourseRecommendationDTO
            {
                CourseId = x.Course.CourseId,
                Title = x.Course.Title,
                CategoryName = x.Course.Genre?.Name,
                Duration = x.Course.Duration,
                Price = x.Course.Price,
                RecommendedOrder = index + 1,
                Confidence = "medium",
                Reason = BuildFallbackReason(x.Course, request)
            })
            .ToList();
    }

    private async Task<List<CourseRecommendationDTO>> GenerateAiRecommendationsAsync(
        CourseRecommendationRequest request,
        List<Course> candidates,
        List<CourseRecommendationDTO> heuristicRecommendations,
        CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_ollamaOptions.BaseUrl);

        var candidatePayload = candidates.Select(c => new
        {
            c.CourseId,
            c.Title,
            c.Description,
            Category = c.Genre?.Name,
            c.Duration,
            c.Price
        });

        var heuristicPayload = heuristicRecommendations.Select(r => new
        {
            r.CourseId,
            r.Title,
            r.RecommendedOrder,
            r.Reason
        });

        var systemPrompt = """
                You are an academic advisor for an e-learning platform.
                Your task is to recommend which courses a learner should study first after selecting a roadmap.
                Prioritize:
                1. beginner-friendly foundation first,
                2. courses that are easier to remember early,
                3. shorter and clearer courses before deep specialization,
                4. logical progression from basic to advanced.

                Return only valid JSON using this shape:
                {
                \"recommendations\": [
                    {
                    \"courseId\": 1,
                    \"recommendedOrder\": 1,
                    \"confidence\": \"high\",
                    \"reason\": \"short explanation\"
                    }
                ]
                }
                Do not include markdown fences or extra text.
                """;

        var userPrompt = $"""
Roadmap: {request.RoadmapName}
Goal: {request.Goal ?? "Not specified"}
Current skill level: {request.CurrentSkillLevel ?? "Not specified"}
Prefer easy to remember first: {request.PreferEasyToRemember}
Number of courses to return: {request.Limit}

Candidate courses:
{JsonSerializer.Serialize(candidatePayload)}

Heuristic baseline:
{JsonSerializer.Serialize(heuristicPayload)}
""";

        var payload = new
        {
            model = _ollamaOptions.Model,
            stream = false,
            format = "json",
            messages = new object[]
            {
                new
                {
                    role = "system",
                    content = systemPrompt
                },
                new
                {
                    role = "user",
                    content = userPrompt
                }
            }
        };

        using var response = await client.PostAsync(
            "api/chat",
            new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"),
            cancellationToken);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(ParseOllamaErrorMessage(response.StatusCode, responseContent));
        }

        var jsonText = ExtractOllamaOutputText(responseContent);
        if (string.IsNullOrWhiteSpace(jsonText))
        {
            return new List<CourseRecommendationDTO>();
        }

        using var recommendationDocument = JsonDocument.Parse(jsonText);
        if (!recommendationDocument.RootElement.TryGetProperty("recommendations", out var recommendationsElement) ||
            recommendationsElement.ValueKind != JsonValueKind.Array)
        {
            return new List<CourseRecommendationDTO>();
        }

        var courseLookup = candidates.ToDictionary(c => c.CourseId);
        var results = new List<CourseRecommendationDTO>();

        foreach (var item in recommendationsElement.EnumerateArray())
        {
            if (!item.TryGetProperty("courseId", out var courseIdElement) ||
                !courseIdElement.TryGetInt32(out var courseId) ||
                !courseLookup.TryGetValue(courseId, out var course))
            {
                continue;
            }

            var recommendedOrder = item.TryGetProperty("recommendedOrder", out var orderElement) &&
                                   orderElement.TryGetInt32(out var parsedOrder)
                ? parsedOrder
                : results.Count + 1;

            results.Add(new CourseRecommendationDTO
            {
                CourseId = course.CourseId,
                Title = course.Title,
                CategoryName = course.Genre?.Name,
                Duration = course.Duration,
                Price = course.Price,
                RecommendedOrder = recommendedOrder,
                Confidence = item.TryGetProperty("confidence", out var confidenceElement)
                    ? confidenceElement.GetString()
                    : "medium",
                Reason = item.TryGetProperty("reason", out var reasonElement)
                    ? reasonElement.GetString()
                    : BuildFallbackReason(course, request)
            });
        }

        return results
            .OrderBy(r => r.RecommendedOrder)
            .Take(request.Limit)
            .ToList();
    }

    private string ExtractOllamaOutputText(string responseContent)
    {
        using var document = JsonDocument.Parse(responseContent);
        if (!document.RootElement.TryGetProperty("message", out var messageElement) ||
            messageElement.ValueKind != JsonValueKind.Object)
        {
            return string.Empty;
        }

        if (messageElement.TryGetProperty("content", out var contentElement))
        {
            return contentElement.GetString() ?? string.Empty;
        }

        return string.Empty;
    }

    private static string ParseOllamaErrorMessage(System.Net.HttpStatusCode statusCode, string responseContent)
    {
        try
        {
            using var document = JsonDocument.Parse(responseContent);
            if (document.RootElement.TryGetProperty("error", out var errorElement))
            {
                if (errorElement.ValueKind == JsonValueKind.String)
                {
                    return $"Ollama error ({(int)statusCode}): {errorElement.GetString()}";
                }
            }
        }
        catch
        {
        }

        return $"Ollama error ({(int)statusCode}).";
    }

    private static string BuildFailureMessage(Exception ex)
    {
        var rawMessage = ex.Message;

        if (rawMessage.Contains("actively refused", StringComparison.OrdinalIgnoreCase) ||
            rawMessage.Contains("Unable to connect", StringComparison.OrdinalIgnoreCase) ||
            rawMessage.Contains("No connection", StringComparison.OrdinalIgnoreCase))
        {
            return "Khong ket noi duoc toi Ollama local. Hay cai Ollama va chay server local, sau do he thong se dung AI thay vi fallback ranking.";
        }

        if (rawMessage.Contains("not found", StringComparison.OrdinalIgnoreCase) ||
            rawMessage.Contains("model", StringComparison.OrdinalIgnoreCase))
        {
            return "Ollama da chay nhung model chua san sang. Hay pull model duoc cau hinh roi test lai. He thong da chuyen sang fallback ranking.";
        }

        return $"AI tam thoi khong kha dung ({rawMessage}). He thong da chuyen sang xep hang theo quy tac noi bo.";
    }

    private bool CanUseAi()
    {
        return _ollamaOptions.EnableRecommendations &&
               !string.IsNullOrWhiteSpace(_ollamaOptions.BaseUrl) &&
               !string.IsNullOrWhiteSpace(_ollamaOptions.Model);
    }

    private static double CalculateHeuristicScore(Course course, CourseRecommendationRequest request)
    {
        var score = 0d;
        var searchSpace = string.Join(
            ' ',
            new[]
            {
                request.RoadmapName,
                request.Goal,
                course.Title,
                course.Description,
                course.Genre?.Name
            }.Where(s => !string.IsNullOrWhiteSpace(s))).ToLowerInvariant();

        foreach (var keyword in Tokenize(request.RoadmapName, request.Goal))
        {
            if (searchSpace.Contains(keyword))
            {
                score += 12;
            }
        }

        var beginnerKeywords = new[]
        {
            "basic", "beginner", "foundation", "intro", "introduction",
            "co ban", "nhap mon", "nen tang"
        };
        var advancedKeywords = new[]
        {
            "advanced", "expert", "master", "chuyen sau", "nang cao"
        };

        var titleAndDescription = $"{course.Title} {course.Description}".ToLowerInvariant();

        if (beginnerKeywords.Any(titleAndDescription.Contains))
        {
            score += 25;
        }

        if (advancedKeywords.Any(titleAndDescription.Contains))
        {
            score -= 10;
        }

        if (request.PreferEasyToRemember)
        {
            score += Math.Max(0, 20 - (course.Duration / 30.0));
        }

        score += Math.Max(0, 10 - (double)(course.Price / 100000m));

        return score;
    }

    private static IEnumerable<string> Tokenize(params string?[] values)
    {
        return string.Join(' ', values.Where(v => !string.IsNullOrWhiteSpace(v)))
            .ToLowerInvariant()
            .Split(new[] { ' ', ',', '.', '-', '_', '/', '\\' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(token => token.Length >= 3)
            .Distinct();
    }

    private static string BuildFallbackReason(Course course, CourseRecommendationRequest request)
    {
        var reasons = new List<string>();
        var titleAndDescription = $"{course.Title} {course.Description}".ToLowerInvariant();

        if (titleAndDescription.Contains("co ban") ||
            titleAndDescription.Contains("nhap mon") ||
            titleAndDescription.Contains("basic") ||
            titleAndDescription.Contains("foundation"))
        {
            reasons.Add("phu hop de lay nen tang ngay tu dau");
        }

        if (request.PreferEasyToRemember && course.Duration <= 180)
        {
            reasons.Add("thoi luong gon hon nen de bat dau va de ghi nho");
        }

        if (!string.IsNullOrWhiteSpace(course.Genre?.Name))
        {
            reasons.Add($"gan voi nhom {course.Genre.Name}");
        }

        if (reasons.Count == 0)
        {
            reasons.Add("duoc xep truoc de tao lo trinh hoc tu co ban den nang cao");
        }

        return string.Join("; ", reasons);
    }
}
