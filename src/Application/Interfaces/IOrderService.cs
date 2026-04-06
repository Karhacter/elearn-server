using elearn_server.Application.Common;
using elearn_server.Application.DTOs;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;
using elearn_server.Domain.Entities;

namespace elearn_server.Application.Interfaces;

public interface IOrderService
{
    Task<ServiceResult<IReadOnlyCollection<OrderResponse>>> GetAllAsync();
    Task<ServiceResult<OrderResponse>> GetByIdAsync(int id);
    Task<ServiceResult<OrderResponse>> CreateAsync(OrderRequest request);
    Task<ServiceResult<OrderResponse>> CreateBasicAsync(Order order);
    Task<ServiceResult<OrderResponse>> UpdateAsync(int id, OrderUpdateDTO request);
    Task<ServiceResult<object>> DeleteAsync(int id);
    Task<ServiceResult<IReadOnlyCollection<OrderResponse>>> GetByUserIdAsync(int userId);
}

public interface IOrderDetailService
{
    Task<ServiceResult<IReadOnlyCollection<OrderDetailResponse>>> GetAllAsync();
    Task<ServiceResult<OrderDetailResponse>> GetByIdAsync(int id);
    Task<ServiceResult<IReadOnlyCollection<OrderDetailResponse>>> GetByOrderIdAsync(int orderId);
    Task<ServiceResult<OrderDetailResponse>> CreateAsync(OrderDetailUpsertRequest request);
    Task<ServiceResult<OrderDetailResponse>> UpdateAsync(int id, OrderDetailUpsertRequest request);
    Task<ServiceResult<object>> DeleteAsync(int id);
}
