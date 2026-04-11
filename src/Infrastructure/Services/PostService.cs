using elearn_server.Application.Common;
using elearn_server.Application.Interfaces;
using elearn_server.Application.Mappings;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;
using elearn_server.Domain.Entities;
using elearn_server.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace elearn_server.Infrastructure.Services;

public class PostService : IPostService
{
    private readonly AppDbContext _context;

    public PostService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<List<PostResponse>>> GetAllPostsAsync()
    {
        var posts = await _context.Set<Post>()
            .Include(p => p.Topic)
            .Where(p => !p.IsDeleted)
            .ToListAsync();
        return ServiceResult<List<PostResponse>>.Ok(posts.Select(p => p.ToResponse()).ToList());
    }

    public async Task<ServiceResult<List<PostResponse>>> GetPostsByTopicIdAsync(int topicId)
    {
        var posts = await _context.Set<Post>()
            .Include(p => p.Topic)
            .Where(p => p.TopicId == topicId && !p.IsDeleted)
            .ToListAsync();
        return ServiceResult<List<PostResponse>>.Ok(posts.Select(p => p.ToResponse()).ToList());
    }

    public async Task<ServiceResult<PostResponse>> GetPostByIdAsync(int id)
    {
        var post = await _context.Set<Post>()
            .Include(p => p.Topic)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        if (post == null) return ServiceResult<PostResponse>.Fail(404, "Post not found.");
        return ServiceResult<PostResponse>.Ok(post.ToResponse());
    }

    public async Task<ServiceResult<PostResponse>> CreatePostAsync(CreatePostRequest request)
    {
        var topicExists = await _context.Set<Topic>().AnyAsync(t => t.Id == request.TopicId && !t.IsDeleted);
        if (!topicExists) return ServiceResult<PostResponse>.Fail(400, "Invalid TopicId.");

        var post = new Post
        {
            Title = request.Title,
            Content = request.Content,
            ThumbnailUrl = request.ThumbnailUrl,
            TopicId = request.TopicId,
            CreatedAt = DateTime.UtcNow
        };
        _context.Set<Post>().Add(post);
        await _context.SaveChangesAsync();
        
        await _context.Entry(post).Reference(p => p.Topic).LoadAsync();
        return ServiceResult<PostResponse>.Created(post.ToResponse());
    }

    public async Task<ServiceResult<PostResponse>> UpdatePostAsync(int id, UpdatePostRequest request)
    {
        var post = await _context.Set<Post>().Include(p => p.Topic).FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        if (post == null) return ServiceResult<PostResponse>.Fail(404, "Post not found.");

        if (post.TopicId != request.TopicId)
        {
            var topicExists = await _context.Set<Topic>().AnyAsync(t => t.Id == request.TopicId && !t.IsDeleted);
            if (!topicExists) return ServiceResult<PostResponse>.Fail(400, "Invalid TopicId.");
        }

        post.Title = request.Title;
        post.Content = request.Content;
        post.ThumbnailUrl = request.ThumbnailUrl;
        post.TopicId = request.TopicId;
        post.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        
        await _context.Entry(post).Reference(p => p.Topic).LoadAsync();
        return ServiceResult<PostResponse>.Ok(post.ToResponse());
    }

    public async Task<ServiceResult<bool>> ToggleSoftDeleteAsync(int id)
    {
        var post = await _context.Set<Post>().FirstOrDefaultAsync(p => p.Id == id);
        if (post == null) return ServiceResult<bool>.Fail(404, "Post not found.");

        post.IsDeleted = !post.IsDeleted;
        post.DeletedAt = post.IsDeleted ? DateTime.UtcNow : null;
        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true, "Post soft delete toggled.");
    }
}
