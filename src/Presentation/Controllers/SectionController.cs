using elearn_server.Application.Requests;
using elearn_server.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace elearn_server.Presentation.Controllers;

[Route("api/course")]
public class SectionController(ICourseService courseService) : ApiControllerBase
{

    [HttpGet("{courseId}/sections")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSections(int courseId) =>
        FromResult(await courseService.GetSectionsAsync(courseId));

    [HttpGet("{courseId}/sections/deleted")]
    [AllowAnonymous]
    public async Task<IActionResult> getDeleted(int courseId) =>
        FromResult(await courseService.GetDeletedSectionsAsync(courseId));

    [HttpPost("{courseId}/sections")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> CreateSection(int courseId, [FromBody] SectionCreateRequest request) =>
           FromResult(await courseService.CreateSectionAsync(courseId, request));


    // Update and delete sections and lessons, and reorder sections and lessons within a course
    [HttpPut("{courseId}/sections/{sectionId}")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> UpdateSection(int courseId, int sectionId, [FromBody] SectionUpdateRequest request) =>
        FromResult(await courseService.UpdateSectionAsync(courseId, sectionId, request));


    [HttpDelete("{courseId}/sections/{sectionId}")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> DeleteSection(int courseId, int sectionId) =>
        FromResult(await courseService.DeleteSectionAsync(courseId, sectionId));


    [HttpPatch("{courseId}/sections/{sectionId}/toggle-soft-delete")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> ToggleSoftDelete(int courseId, int sectionId) =>
        FromResult(await courseService.ToggleSectionSoftDeleteAsync(courseId, sectionId));



    // Reorder sections within a course
    [HttpPost("{courseId}/sections/reorder")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> ReorderSections(int courseId, [FromBody] SectionReorderRequest request) =>
        FromResult(await courseService.ReorderSectionsAsync(courseId, request));


    [HttpPost("{courseId}/sections/bulk-soft-delete")]
    public async Task<IActionResult> BulkSoftDelete(int courseId, [FromBody] BulkSoftDeleteRequest request) =>
        FromResult(await courseService.BulkSoftDeleteSectionsAsync(courseId, request));

}