using elearn_server.Application.Common;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;

namespace elearn_server.Application.Interfaces;

public interface IPaymentService
{
    Task<ServiceResult<IReadOnlyCollection<PaymentResponse>>> GetByUserIdAsync(int userId);
    Task<ServiceResult<PaymentResponse>> CreateAsync(PaymentCreateRequest request);
    Task<ServiceResult<object>> ConfirmPaymentAsync(int orderId);
}