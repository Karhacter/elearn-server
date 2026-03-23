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
    public async Task<IActionResult> GetAllUsers() => FromResult(await userService.GetAllAsync());

    [HttpGet("detail/{id}")]
    public async Task<IActionResult> GetUserById(int id) => FromResult(await userService.GetByIdAsync(id));

    [HttpPost("add")]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateDTO userDto) => FromResult(await userService.CreateAsync(userDto));

    [HttpPut("edit/{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateRequest request) => FromResult(await userService.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id) => FromResult(await userService.DeleteAsync(id));

    [HttpPatch("{id}/image")]
    public async Task<IActionResult> UpdateUserImage(int id, [FromBody] UpdateImageRequest request) => FromResult(await userService.UpdateImageAsync(id, request.ImageUrl));

    [HttpPost("{id}/upload-image")]
    public async Task<IActionResult> UploadUserImage(int id, IFormFile imageFile, CancellationToken cancellationToken) =>
        FromResult(await userService.UploadImageAsync(id, imageFile, cancellationToken));
}
