using elearn_server.Application.Requests;
using elearn_server.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using elearn_server.Application.Common;
using elearn_server.Application.DTOs;
namespace elearn_server.Presentation.Controllers;

[Route("api/[controller]")]
[Authorize(Roles = "Student,Instructor,Admin")]
public class RecommendationController(ICourseRecommendationService courseRecommendationService) : ApiControllerBase
{
    [HttpPost("roadmap-courses")]
    public async Task<IActionResult> RecommendCoursesForRoadmap([FromBody] CourseRecommendationRequest request, CancellationToken cancellationToken)
    {
        var result = await courseRecommendationService.RecommendCoursesAsync(request, cancellationToken);
        return FromResult(ServiceResult<CourseRecommendationResponseDTO>.Ok(result));
    }
}
