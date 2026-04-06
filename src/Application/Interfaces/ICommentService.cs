using elearn_server.Application.Common;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;

namespace elearn_server.Application.Interfaces;

public interface ICommentService
{
    Task<ServiceResult<IReadOnlyCollection<CommentResponse>>> GetAllAsync();
    Task<ServiceResult<CommentResponse>> CreateAsync(CommentCreateRequest request);
    Task<ServiceResult<object>> DeleteAsync(int id);
    Task<ServiceResult<ReviewResponse>> CreateOrUpdateReviewAsync(int userId, int courseId, ReviewUpsertRequest request);
    Task<ServiceResult<IReadOnlyCollection<ReviewResponse>>> GetCourseReviewsAsync(int courseId, int? star, string? sortBy);
    Task<ServiceResult<CourseRatingSummaryResponse>> GetCourseRatingSummaryAsync(int courseId);
    Task<ServiceResult<ReviewResponse>> ReplyToReviewAsync(int ratingId, ReviewReplyRequest request);
    Task<ServiceResult<ReviewResponse>> ModerateReviewAsync(int ratingId, ReviewModerationRequest request);
}