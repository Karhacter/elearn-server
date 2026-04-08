using elearn_server.Application.Requests;
using elearn_server.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace elearn_server.Presentation.Controllers;

[Route("api/sections")]
public class LessonController(ICourseService courseService) : ApiControllerBase
{
    // Get lesson with Section
    [HttpGet("{sectionId}/lessons")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLessons(int sectionId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
            FromResult(await courseService.GetLessonsAsync(sectionId, page, pageSize));

    // Create lesson
    [HttpPost("{sectionId}/lessons")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> CreateLesson(int sectionId, [FromBody] LessonCreateRequest request) =>
        FromResult(await courseService.CreateLessonAsync(sectionId, request));


    // Update  lessons
    [HttpPut("{sectionId}/lessons/{lessonId}")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> UpdateLesson(int sectionId, int lessonId, [FromBody] LessonUpdateRequest request) =>
        FromResult(await courseService.UpdateLessonAsync(sectionId, lessonId, request));


    // Delete a lesson from a section
    [HttpDelete("{sectionId}/lessons/{lessonId}")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> DeleteLesson(int sectionId, int lessonId) =>
        FromResult(await courseService.DeleteLessonAsync(sectionId, lessonId));

    // Change between soft-delete and restore
    [HttpPatch("{sectionId}/lessons/{lessonId}/toggle-soft-delete")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> ToggleLessonSoftDelete(int sectionId, int lessonId) =>
        FromResult(await courseService.ToggleLessonSoftDeleteAsync(sectionId, lessonId));


    // Reorder lessons within a section - sap xep lai dua tren order
    [HttpPost("{sectionId}/lessons/reorder")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> ReorderLessons(int sectionId, [FromBody] LessonReorderRequest request) =>
        FromResult(await courseService.ReorderLessonsAsync(sectionId, request));
}