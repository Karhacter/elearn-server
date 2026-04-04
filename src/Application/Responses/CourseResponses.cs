namespace elearn_server.Application.Responses;

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
