using elearn_server.Application.Common;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;

namespace elearn_server.Application.Interfaces;

public interface IMenuService
{
    Task<ServiceResult<List<MenuResponse>>> GetAllMenusAsync();
    Task<ServiceResult<List<MenuResponse>>> GetMenuTreeAsync();
    Task<ServiceResult<MenuResponse>> GetMenuByIdAsync(int id);
    Task<ServiceResult<MenuResponse>> CreateMenuAsync(CreateMenuRequest request);
    Task<ServiceResult<MenuResponse>> UpdateMenuAsync(int id, UpdateMenuRequest request);
    Task<ServiceResult<bool>> ToggleSoftDeleteAsync(int id);
}
