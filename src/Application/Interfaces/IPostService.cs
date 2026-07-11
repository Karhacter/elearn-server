using elearn_server.Application.Common;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;

namespace elearn_server.Application.Interfaces;

public interface IPostService
{
    Task<ServiceResult<List<PostResponse>>> GetAllPostsAsync(int page = 1, int limit = 10);
    Task<ServiceResult<List<PostResponse>>> GetPostsByTopicIdAsync(int topicId, int page = 1, int limit = 10);

    Task<ServiceResult<PostResponse>> GetPostBySlug(string slug);

    Task<ServiceResult<PostResponse>> GetPostByIdAsync(int id);
    Task<ServiceResult<PostResponse>> CreatePostAsync(CreatePostRequest request);
    Task<ServiceResult<PostResponse>> UpdatePostAsync(int id, UpdatePostRequest request);
    Task<ServiceResult<bool>> ToggleSoftDeleteAsync(int id);
}
