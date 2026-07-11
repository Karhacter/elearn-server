using elearn_server.Application.Common;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;

namespace elearn_server.Application.Interfaces;

public interface ICheckoutService
{
    Task<ServiceResult<CheckoutResponse>> CheckoutFromCartAsync(int userId, CheckoutRequest request, string ipAddress);
}
