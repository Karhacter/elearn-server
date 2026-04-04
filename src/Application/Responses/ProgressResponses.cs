namespace elearn_server.Application.Responses;

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
