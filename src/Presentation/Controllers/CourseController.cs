using elearn_server.Application.Requests;
using elearn_server.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace elearn_server.Presentation.Controllers;

[Route("api/[controller]")]
public class CourseController(ICourseService courseService) : ApiControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllCourses() => FromResult(await courseService.GetAllAsync());

    [HttpGet("paged")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPagedCourses(int pageNumber = 1, int pageSize = 10) => FromResult(await courseService.GetPagedAsync(pageNumber, pageSize));

    [HttpGet("detail/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCourseById(int id) => FromResult(await courseService.GetByIdAsync(id));

    [HttpPost("add")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> CreateCourse([FromBody] CourseUpsertRequest request) => FromResult(await courseService.CreateAsync(request));

    [HttpPut("edit/{id}")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseUpsertRequest request) => FromResult(await courseService.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCourse(int id) => FromResult(await courseService.DeleteAsync(id));

    [HttpGet("category/{categoryId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCoursesByCategoryId(int categoryId) => FromResult(await courseService.GetByCategoryIdAsync(categoryId));

    [HttpPatch("{id}/image")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> UpdateCourseImage(int id, [FromBody] UpdateImageRequest request) => FromResult(await courseService.UpdateImageAsync(id, request.ImageUrl));

    [HttpPost("{id}/upload-image")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> UploadCourseImage(int id, IFormFile imageFile, CancellationToken cancellationToken) =>
        FromResult(await courseService.UploadImageAsync(id, imageFile, cancellationToken));

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> SearchCourses([FromQuery] string? keyword, [FromQuery] int? genreId, [FromQuery] int? instructorId) =>
        FromResult(await courseService.SearchAsync(keyword, genreId, instructorId));

    [HttpGet("{id}/preview")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> PreviewCourse(int id) => FromResult(await courseService.PreviewAsync(id));

    [HttpPost("{id}/publish")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> PublishCourse(int id) =>
        FromResult(await courseService.PublishAsync(id, User.IsInRole("Admin")));

    [HttpPost("{id}/unpublish")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> UnpublishCourse(int id) =>
        FromResult(await courseService.UnpublishAsync(id));

    [HttpPost("{courseId}/sections")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> CreateSection(int courseId, [FromBody] SectionCreateRequest request) =>
        FromResult(await courseService.CreateSectionAsync(courseId, request));

    [HttpPut("{courseId}/sections/{sectionId}")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> UpdateSection(int courseId, int sectionId, [FromBody] SectionUpdateRequest request) =>
        FromResult(await courseService.UpdateSectionAsync(courseId, sectionId, request));

    [HttpDelete("{courseId}/sections/{sectionId}")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> DeleteSection(int courseId, int sectionId) =>
        FromResult(await courseService.DeleteSectionAsync(courseId, sectionId));

    [HttpPost("{courseId}/sections/reorder")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> ReorderSections(int courseId, [FromBody] SectionReorderRequest request) =>
        FromResult(await courseService.ReorderSectionsAsync(courseId, request));

    [HttpPost("{courseId}/sections/{sectionId}/lessons")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> CreateLesson(int courseId, int sectionId, [FromBody] LessonCreateRequest request) =>
        FromResult(await courseService.CreateLessonAsync(courseId, sectionId, request));

    [HttpPut("{courseId}/sections/{sectionId}/lessons/{lessonId}")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> UpdateLesson(int courseId, int sectionId, int lessonId, [FromBody] LessonUpdateRequest request) =>
        FromResult(await courseService.UpdateLessonAsync(courseId, sectionId, lessonId, request));

    [HttpDelete("{courseId}/sections/{sectionId}/lessons/{lessonId}")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> DeleteLesson(int courseId, int sectionId, int lessonId) =>
        FromResult(await courseService.DeleteLessonAsync(courseId, sectionId, lessonId));

    [HttpPost("{courseId}/sections/{sectionId}/lessons/reorder")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> ReorderLessons(int courseId, int sectionId, [FromBody] LessonReorderRequest request) =>
        FromResult(await courseService.ReorderLessonsAsync(courseId, sectionId, request));
}
