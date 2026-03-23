using System.ComponentModel.DataAnnotations;

namespace elearn_server.Application.Requests;

public class CourseRecommendationRequest
{
    [Required]
    [MaxLength(150)]
    public string RoadmapName { get; set; } = string.Empty;

    public int? CategoryId { get; set; }

    [MaxLength(300)]
    public string? Goal { get; set; }

    [MaxLength(50)]
    public string? CurrentSkillLevel { get; set; }

    public bool PreferEasyToRemember { get; set; } = true;

    [Range(1, 10)]
    public int Limit { get; set; } = 5;
}
