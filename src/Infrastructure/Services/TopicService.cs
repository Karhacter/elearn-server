using elearn_server.Application.Common;
using elearn_server.Application.Interfaces;
using elearn_server.Application.Mappings;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;
using elearn_server.Domain.Entities;
using elearn_server.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace elearn_server.Infrastructure.Services;

public class TopicService : ITopicService
{
    private readonly AppDbContext _context;

    public TopicService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<List<TopicResponse>>> GetAllTopicsAsync()
    {
        var topics = await _context.Set<Topic>()
            .Where(t => !t.IsDeleted)
            .ToListAsync();
        return ServiceResult<List<TopicResponse>>.Ok(topics.Select(t => t.ToResponse()).ToList());
    }

    public async Task<ServiceResult<TopicResponse>> GetTopicByIdAsync(int id)
    {
        var topic = await _context.Set<Topic>().FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        if (topic == null) return ServiceResult<TopicResponse>.Fail(404, "Topic not found.");
        return ServiceResult<TopicResponse>.Ok(topic.ToResponse());
    }

    public async Task<ServiceResult<TopicResponse>> CreateTopicAsync(CreateTopicRequest request)
    {
        var topic = new Topic
        {
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };
        _context.Set<Topic>().Add(topic);
        await _context.SaveChangesAsync();
        return ServiceResult<TopicResponse>.Created(topic.ToResponse());
    }

    public async Task<ServiceResult<TopicResponse>> UpdateTopicAsync(int id, UpdateTopicRequest request)
    {
        var topic = await _context.Set<Topic>().FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        if (topic == null) return ServiceResult<TopicResponse>.Fail(404, "Topic not found.");

        topic.Name = request.Name;
        topic.Description = request.Description;
        topic.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return ServiceResult<TopicResponse>.Ok(topic.ToResponse());
    }

    public async Task<ServiceResult<bool>> ToggleSoftDeleteAsync(int id)
    {
        var topic = await _context.Set<Topic>().FirstOrDefaultAsync(t => t.Id == id);
        if (topic == null) return ServiceResult<bool>.Fail(404, "Topic not found.");

        topic.IsDeleted = !topic.IsDeleted;
        topic.DeletedAt = topic.IsDeleted ? DateTime.UtcNow : null;
        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true, "Topic soft delete toggled.");
    }
}
