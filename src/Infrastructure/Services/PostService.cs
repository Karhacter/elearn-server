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

    public async Task<ServiceResult<List<PostResponse>>> GetAllPostsAsync(int page = 1, int limit = 10)
    {
        var posts = await _context.Set<Post>()
            .Include(p => p.Topic)
            .Where(p => !p.IsDeleted)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();
        return ServiceResult<List<PostResponse>>.Ok(posts.Select(p => p.ToResponse()).ToList());
    }

    public async Task<ServiceResult<List<PostResponse>>> GetPostsByTopicIdAsync(int topicId, int page = 1, int limit = 10)
    {
        var posts = await _context.Set<Post>()
            .Include(p => p.Topic)
            .Where(p => p.TopicId == topicId && !p.IsDeleted)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();
        return ServiceResult<List<PostResponse>>.Ok(posts.Select(p => p.ToResponse()).ToList());
    }

    public async Task<ServiceResult<PostResponse>> GetPostBySlug(string slug)
    {
        var query = _context.Set<Post>().Include(p => p.Topic).AsQueryable();
        Post? post = null;
        
        if (int.TryParse(slug, out var id))
        {
            post = await query.FirstOrDefaultAsync(p => (p.Slug == slug || p.Id == id) && !p.IsDeleted);
        }
        else
        {
            post = await query.FirstOrDefaultAsync(p => p.Slug == slug && !p.IsDeleted);
        }

        if (post == null) return ServiceResult<PostResponse>.Fail(404, "Post not found.");
        return ServiceResult<PostResponse>.Ok(post.ToResponse());
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
            Slug = await GenerateUniqueSlugAsync(request.Slug, request.Title),
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
        post.Slug = await GenerateUniqueSlugAsync(request.Slug, request.Title, post.Id);
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

    private async Task<string> GenerateUniqueSlugAsync(string? requestedSlug, string title, int? ignorePostId = null)
    {
        var baseSlug = NormalizeSlug(string.IsNullOrWhiteSpace(requestedSlug) ? title : requestedSlug);
        if (string.IsNullOrWhiteSpace(baseSlug))
        {
            baseSlug = "post";
        }

        var slug = baseSlug;
        var suffix = 1;
        while (true)
        {
            var existing = await _context.Set<Post>().FirstOrDefaultAsync(p => p.Slug == slug && !p.IsDeleted);
            if (existing is null || (ignorePostId.HasValue && existing.Id == ignorePostId.Value))
            {
                return slug;
            }

            slug = $"{baseSlug}-{suffix++}";
        }
    }

    private static string NormalizeSlug(string value)
    {
        var slug = value.Trim().ToLowerInvariant();
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\-]", string.Empty);
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\-{2,}", "-").Trim('-');
        return slug;
    }
}
