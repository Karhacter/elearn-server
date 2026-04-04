using elearn_server.Application.Requests;
using elearn_server.Application.DTOs;
using elearn_server.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace elearn_server.Presentation.Controllers;

[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UserController(IUserService userService) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
        FromResult(await userService.GetAllAsync(page, pageSize));

    // get Deleted Users
    [HttpGet("deleted")]
    public async Task<IActionResult> GetDeletedUsers() => FromResult(await userService.GetDeletedAsync());
    
    [HttpGet("detail/{id}")]
    public async Task<IActionResult> GetUserById(int id) => FromResult(await userService.GetByIdAsync(id));

    [HttpPost("add")]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateDTO userDto) => FromResult(await userService.CreateAsync(userDto));

    [HttpPut("edit/{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateRequest request) => FromResult(await userService.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id) => FromResult(await userService.DeleteAsync(id));

    // soft-delete
    [HttpPatch("{id}/toggle-soft-delete")]
    public async Task<IActionResult> ToggleSoftDelete(int id) => FromResult(await userService.ToggleSoftDeleteAsync(id));

    [HttpPost("bulk-soft-delete")]
    public async Task<IActionResult> BulkSoftDelete([FromBody] BulkSoftDeleteRequest request) =>
        FromResult(await userService.BulkSoftDeleteAsync(request));

    [HttpPatch("{id}/image")]
    public async Task<IActionResult> UpdateUserImage(int id, [FromBody] UpdateImageRequest request) => FromResult(await userService.UpdateImageAsync(id, request.ImageUrl));

    [HttpPost("{id}/upload-image")]
    public async Task<IActionResult> UploadUserImage(int id, IFormFile imageFile, CancellationToken cancellationToken) =>
        FromResult(await userService.UploadImageAsync(id, imageFile, cancellationToken));
}
