using elearn_server.Application.Common;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;

namespace elearn_server.Application.Interfaces;


public interface ICourseService
{
    Task<ServiceResult<PagedResult<CourseResponse>>> GetAllAsync(int page, int pageSize);

    Task<ServiceResult<PagedResult<CourseResponse>>> GetPagedAsync(int pageNumber, int pageSize);

    Task<ServiceResult<PagedResult<CourseResponse>>> GetDeletedAsync(int page, int pageSize);
    Task<ServiceResult<PagedResult<CourseClientResponse>>> GetClientPagedAsync(int pageNumber, int pageSize);
    Task<ServiceResult<CourseResponse>> GetByIdAsync(int id);
    Task<ServiceResult<CourseResponse>> CreateAsync(CourseCreateRequest request);
    Task<ServiceResult<CourseResponse>> UpdateAsync(int id, CourseCreateRequest request);
    Task<ServiceResult<object>> DeleteAsync(int id);
    Task<ServiceResult<CourseResponse>> ToggleSoftDeleteAsync(int id);
    Task<ServiceResult<BulkSoftDeleteResponse>> BulkSoftDeleteAsync(BulkSoftDeleteRequest request);
    Task<ServiceResult<IReadOnlyCollection<CourseResponse>>> GetByCategoryIdAsync(int categoryId);
    Task<ServiceResult<ImageUploadResponse>> UpdateImageAsync(int id, string imageUrl);
    Task<ServiceResult<ImageUploadResponse>> UploadImageAsync(int id, IFormFile imageFile, CancellationToken cancellationToken);
    Task<ServiceResult<IReadOnlyCollection<CourseResponse>>> SearchAsync(string? keyword, int? genreId, int? instructorId);
    Task<ServiceResult<CoursePreviewResponse>> PreviewAsync(int courseId);
    Task<ServiceResult<CourseResponse>> PublishAsync(int courseId, bool isAdmin);
    Task<ServiceResult<CourseResponse>> UnpublishAsync(int courseId);


    // For Sections
    Task<ServiceResult<IReadOnlyCollection<SectionResponse>>> GetSectionsAsync(int courseId);
    Task<ServiceResult<IReadOnlyCollection<SectionResponse>>> GetDeletedSectionsAsync(int courseId);

    Task<ServiceResult<BulkSoftDeleteResponse>> BulkSoftDeleteSectionsAsync(int courseId, BulkSoftDeleteRequest request);
    Task<ServiceResult<SectionResponse>> CreateSectionAsync(int courseId, SectionCreateRequest request);
    Task<ServiceResult<SectionResponse>> UpdateSectionAsync(int courseId, int sectionId, SectionUpdateRequest request);
    Task<ServiceResult<object>> DeleteSectionAsync(int courseId, int sectionId);
    Task<ServiceResult<SectionResponse>> ToggleSectionSoftDeleteAsync(int courseId, int sectionId);
    Task<ServiceResult<IReadOnlyCollection<SectionResponse>>> ReorderSectionsAsync(int courseId, SectionReorderRequest request);


    // For Lessons
    Task<ServiceResult<PagedResult<LessonResponse>>> GetLessonsAsync(int sectionId, int page, int pageSize);
    Task<ServiceResult<LessonResponse>> CreateLessonAsync(int sectionId, LessonCreateRequest request);
    Task<ServiceResult<LessonResponse>> UpdateLessonAsync(int sectionId, int lessonId, LessonUpdateRequest request);
    Task<ServiceResult<object>> DeleteLessonAsync(int sectionId, int lessonId);
    Task<ServiceResult<LessonResponse>> ToggleLessonSoftDeleteAsync(int sectionId, int lessonId);

    Task<ServiceResult<PagedResult<LessonResponse>>> GetDeletedLessonsAsync(int sectionId, int page, int pageSize);

    Task<ServiceResult<BulkSoftDeleteResponse>> BulkSoftDeleteLessonsAsync(int sectionId, BulkSoftDeleteRequest request);
    Task<ServiceResult<IReadOnlyCollection<LessonResponse>>> ReorderLessonsAsync(int sectionId, LessonReorderRequest request);
}
