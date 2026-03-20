using elearn_server.DTO;
using elearn_server.Request;

namespace elearn_server.Services;

public interface ICourseRecommendationService
{
    Task<CourseRecommendationResponseDTO> RecommendCoursesAsync(
        CourseRecommendationRequest request,
        CancellationToken cancellationToken = default);
}
