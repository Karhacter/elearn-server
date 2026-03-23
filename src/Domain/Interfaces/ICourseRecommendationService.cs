using elearn_server.Application.DTOs;
using elearn_server.Application.Requests;

namespace elearn_server.Application.Interfaces;

public interface ICourseRecommendationService
{
    Task<CourseRecommendationResponseDTO> RecommendCoursesAsync(
        CourseRecommendationRequest request,
        CancellationToken cancellationToken = default);
}
