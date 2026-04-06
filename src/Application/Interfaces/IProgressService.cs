using elearn_server.Application.Common;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;

namespace elearn_server.Application.Interfaces;

public interface IProgressService
{
    Task<ServiceResult<IReadOnlyCollection<MyLearningItemResponse>>> GetMyLearningAsync(int userId);
    Task<ServiceResult<CourseProgressResponse>> GetCourseProgressAsync(int userId, int courseId);
    Task<ServiceResult<LessonProgressResponse>> CompleteLessonAsync(int userId, int lessonId);
    Task<ServiceResult<LessonProgressResponse>> UpdateResumePositionAsync(int userId, int lessonId, LessonResumePositionRequest request);
}
