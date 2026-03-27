using elearn_server.Application.Requests;
using elearn_server.Application.Responses;
using elearn_server.Application.Mappings;
using elearn_server.Domain.Entities;
using elearn_server.Infrastructure.Persistence.Repositories;
using elearn_server.Application.Common;
using elearn_server.Domain.Enums;

using elearn_server.Application.Interfaces;
namespace elearn_server.Infrastructure.Services.Commerce;

public class PaymentService(IPaymentRepository repository) : IPaymentService
{
    public async Task<ServiceResult<IReadOnlyCollection<PaymentResponse>>> GetByUserIdAsync(int userId) =>
        ServiceResult<IReadOnlyCollection<PaymentResponse>>.Ok((await repository.GetByUserIdAsync(userId)).Select(p => p.ToResponse()).ToList());

    public async Task<ServiceResult<PaymentResponse>> CreateAsync(PaymentCreateRequest request)
    {
        if (await repository.GetUserAsync(request.UserId) is null)
        {
            return ServiceResult<PaymentResponse>.Fail(StatusCodes.Status404NotFound, "User not found.");
        }
        var course = await repository.GetCourseAsync(request.CourseId);
        if (course is null)
        {
            return ServiceResult<PaymentResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.");
        }

        var payment = new Payment
        {
            UserId = request.UserId,
            CourseId = request.CourseId,
            Amount = request.Amount,
            Method = request.Method.Trim(),
            PaymentDate = DateTime.UtcNow,
            Course = course
        };

        await repository.AddAsync(payment);
        await repository.SaveChangesAsync();
        return ServiceResult<PaymentResponse>.Created(payment.ToResponse(), "Payment processed successfully.");
    }

    public async Task<ServiceResult<object>> ConfirmPaymentAsync(int orderId)
    {
        var order = await repository.GetOrderAsync(orderId);
        if (order is null)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status404NotFound, "Đơn hàng không tồn tại.");
        }
        if (order.Status == OrderStatus.Completed)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status400BadRequest, "Đơn hàng đã được thanh toán.");
        }
        order.Status = OrderStatus.Completed;
        await repository.SaveChangesAsync();
        return ServiceResult<object>.Ok(new { orderId = order.OrderID }, "Thanh toán thành công. Đơn hàng đã được hoàn tất.");
    }
}
