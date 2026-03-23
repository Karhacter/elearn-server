namespace elearn_server.Application.DTOs;

public class CourseRecommendationDTO
{
    public int CourseId { get; set; }
    public string? Title { get; set; }
    public string? CategoryName { get; set; }
    public int Duration { get; set; }
    public decimal Price { get; set; }
    public int RecommendedOrder { get; set; }
    public string? Reason { get; set; }
    public string? Confidence { get; set; }
}

public class CourseRecommendationResponseDTO
{
    public string RoadmapName { get; set; } = string.Empty;
    public string? Goal { get; set; }
    public bool UsedAi { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public List<CourseRecommendationDTO> Recommendations { get; set; } = new();
}
