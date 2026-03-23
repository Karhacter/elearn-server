using elearn_server.Application.Requests;
using elearn_server.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace elearn_server.Presentation.Controllers;

[Route("api/[controller]")]
public class CommentController(ICommentService commentService) : ApiControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllComments() => FromResult(await commentService.GetAllAsync());

    [HttpPost]
    [Authorize(Roles = "Student,Instructor,Admin")]
    public async Task<IActionResult> AddComment([FromBody] CommentCreateRequest request) => FromResult(await commentService.CreateAsync(request));

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Student,Instructor")]
    public async Task<IActionResult> DeleteComment(int id) => FromResult(await commentService.DeleteAsync(id));
}
