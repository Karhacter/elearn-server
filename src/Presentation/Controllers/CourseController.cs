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
    public async Task<IActionResult> GetAllCourses([FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
        FromResult(await courseService.GetAllAsync(page, pageSize));

    // get Deleted Course
    [HttpGet("deleted")]
    public async Task<IActionResult> GetDeletedCourses([FromQuery] int page = 1, [FromQuery] int pageSize = 10) => FromResult(await courseService.GetDeletedAsync(page, pageSize));


    [HttpGet("detail/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCourseById(int id) => FromResult(await courseService.GetByIdAsync(id));

    [HttpPost("add")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> CreateCourse([FromBody] CourseCreateRequest request) => FromResult(await courseService.CreateAsync(request));

    [HttpPut("edit/{id}")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseCreateRequest request) => FromResult(await courseService.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCourse(int id) => FromResult(await courseService.DeleteAsync(id));

    [HttpPatch("{id}/toggle-soft-delete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleSoftDelete(int id) => FromResult(await courseService.ToggleSoftDeleteAsync(id));

    [HttpPost("bulk-soft-delete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> BulkSoftDelete([FromBody] BulkSoftDeleteRequest request) =>
        FromResult(await courseService.BulkSoftDeleteAsync(request));

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


    // Preview course for instructors and admins before publishing
    [HttpGet("{id}/preview")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> PreviewCourse(int id) => FromResult(await courseService.PreviewAsync(id));


    // Publish and unpublish courses (only admins can publish, instructors can only unpublish their own courses)
    // Giảng viên gửi lên / admin duyệt 
    [HttpPost("{id}/publish")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> PublishCourse(int id) =>
        FromResult(await courseService.PublishAsync(id, User.IsInRole("Admin")));

    // Từ chối
    [HttpPost("{id}/unpublish")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> UnpublishCourse(int id) =>
        FromResult(await courseService.UnpublishAsync(id));


    // Add, update, and delete sections and lessons within a course
   
}
