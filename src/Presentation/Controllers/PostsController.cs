using elearn_server.Application.Interfaces;
using elearn_server.Application.Requests;
using Microsoft.AspNetCore.Mvc;

namespace elearn_server.Presentation.Controllers;

[Route("api/posts")]
public class PostsController : ApiControllerBase
{
    private readonly IPostService _postService;

    public PostsController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPosts([FromQuery] int? topicId)
    {
        if (topicId.HasValue)
        {
            return FromResult(await _postService.GetPostsByTopicIdAsync(topicId.Value));
        }
        return FromResult(await _postService.GetAllPostsAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostById(int id)
    {
        return FromResult(await _postService.GetPostByIdAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
    {
        return FromResult(await _postService.CreatePostAsync(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdatePostRequest request)
    {
        return FromResult(await _postService.UpdatePostAsync(id, request));
    }

    [HttpPatch("{id}/toggle-soft-delete")]
    public async Task<IActionResult> ToggleSoftDelete(int id)
    {
        return FromResult(await _postService.ToggleSoftDeleteAsync(id));
    }
}
