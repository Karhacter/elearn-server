using elearn_server.Application.Interfaces;
using elearn_server.Application.Requests;
using Microsoft.AspNetCore.Mvc;

namespace elearn_server.Presentation.Controllers;

[Route("api/topics")]
public class TopicsController : ApiControllerBase
{
    private readonly ITopicService _topicService;

    public TopicsController(ITopicService topicService)
    {
        _topicService = topicService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTopics()
    {
        return FromResult(await _topicService.GetAllTopicsAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTopicById(int id)
    {
        return FromResult(await _topicService.GetTopicByIdAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTopic([FromBody] CreateTopicRequest request)
    {
        return FromResult(await _topicService.CreateTopicAsync(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTopic(int id, [FromBody] UpdateTopicRequest request)
    {
        return FromResult(await _topicService.UpdateTopicAsync(id, request));
    }

    [HttpPatch("{id}/toggle-soft-delete")]
    public async Task<IActionResult> ToggleSoftDelete(int id)
    {
        return FromResult(await _topicService.ToggleSoftDeleteAsync(id));
    }
}
