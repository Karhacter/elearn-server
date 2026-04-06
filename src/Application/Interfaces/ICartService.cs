using elearn_server.Application.Common;
using elearn_server.Application.DTOs;
using elearn_server.Application.Requests;

namespace elearn_server.Application.Interfaces;

public interface ICartService
{
    Task<ServiceResult<CartDTO>> GetCartByUserAsync(int userId, bool autoCreate);
    Task<ServiceResult<object>> InitCartsAsync();
    Task<ServiceResult<CartDTO>> AddToCartAsync(AddToCartRequest request);
    Task<ServiceResult<CartDTO>> UpdateCartItemAsync(UpdateCartItemRequest request);
    Task<ServiceResult<CartDTO>> RemoveItemAsync(RemoveCartItemRequest request);
    Task<ServiceResult<CartDTO>> ClearCartAsync(int userId);
}