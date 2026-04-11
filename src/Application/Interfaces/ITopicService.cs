using elearn_server.Application.Common;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;

namespace elearn_server.Application.Interfaces;

public interface ITopicService
{
    Task<ServiceResult<List<TopicResponse>>> GetAllTopicsAsync();
    Task<ServiceResult<TopicResponse>> GetTopicByIdAsync(int id);
    Task<ServiceResult<TopicResponse>> CreateTopicAsync(CreateTopicRequest request);
    Task<ServiceResult<TopicResponse>> UpdateTopicAsync(int id, UpdateTopicRequest request);
    Task<ServiceResult<bool>> ToggleSoftDeleteAsync(int id);
}
