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
}
