using elearn_server.Application.Requests;
using elearn_server.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace elearn_server.Presentation.Controllers;

[Route("api/course/section")]
public class LessonController(ICourseService courseService) : ApiControllerBase
{
    [HttpGet("{courseId}/sections/{sectionId}/lessons")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLessons(int courseId, int sectionId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
            FromResult(await courseService.GetLessonsAsync(courseId, sectionId, page, pageSize));

    [HttpPost("{courseId}/sections/{sectionId}/lessons")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> CreateLesson(int courseId, int sectionId, [FromBody] LessonCreateRequest request) =>
        FromResult(await courseService.CreateLessonAsync(courseId, sectionId, request));


    // Update  lessons
    [HttpPut("{courseId}/sections/{sectionId}/lessons/{lessonId}")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> UpdateLesson(int courseId, int sectionId, int lessonId, [FromBody] LessonUpdateRequest request) =>
        FromResult(await courseService.UpdateLessonAsync(courseId, sectionId, lessonId, request));


    // Delete a lesson from a section
    [HttpDelete("{courseId}/sections/{sectionId}/lessons/{lessonId}")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> DeleteLesson(int courseId, int sectionId, int lessonId) =>
        FromResult(await courseService.DeleteLessonAsync(courseId, sectionId, lessonId));

    [HttpPatch("{courseId}/sections/{sectionId}/lessons/{lessonId}/toggle-soft-delete")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> ToggleLessonSoftDelete(int courseId, int sectionId, int lessonId) =>
        FromResult(await courseService.ToggleLessonSoftDeleteAsync(courseId, sectionId, lessonId));

    // Reorder lessons within a section
    [HttpPost("{courseId}/sections/{sectionId}/lessons/reorder")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> ReorderLessons(int courseId, int sectionId, [FromBody] LessonReorderRequest request) =>
        FromResult(await courseService.ReorderLessonsAsync(courseId, sectionId, request));
}