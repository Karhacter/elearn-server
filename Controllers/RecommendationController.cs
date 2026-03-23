using elearn_server.Request;
using elearn_server.Services;
using Microsoft.AspNetCore.Mvc;

namespace elearn_server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecommendationController : ControllerBase
{
    private readonly ICourseRecommendationService _courseRecommendationService;

    public RecommendationController(ICourseRecommendationService courseRecommendationService)
    {
        _courseRecommendationService = courseRecommendationService;
    }

    [HttpPost("roadmap-courses")]
    public async Task<IActionResult> RecommendCoursesForRoadmap(
        [FromBody] CourseRecommendationRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var result = await _courseRecommendationService.RecommendCoursesAsync(request, cancellationToken);
        return Ok(result);
    }
}
