using elearn_server.Application.Requests;
using elearn_server.Application.Responses;
using elearn_server.Application.Mappings;
using elearn_server.Domain.Entities;
using elearn_server.Infrastructure.Persistence.Repositories;
using elearn_server.Application.Common;
using elearn_server.Application.Interfaces;
namespace elearn_server.Infrastructure.Services.Commerce;

public class OrderDetailService(IOrderDetailRepository repository) : IOrderDetailService
{
    public async Task<ServiceResult<IReadOnlyCollection<OrderDetailResponse>>> GetAllAsync() =>
        ServiceResult<IReadOnlyCollection<OrderDetailResponse>>.Ok((await repository.GetAllAsync()).Select(d => d.ToResponse()).ToList());

    public async Task<ServiceResult<OrderDetailResponse>> GetByIdAsync(int id)
    {
        var detail = await repository.GetByIdAsync(id);
        return detail is null
            ? ServiceResult<OrderDetailResponse>.Fail(StatusCodes.Status404NotFound, "Order detail not found.")
            : ServiceResult<OrderDetailResponse>.Ok(detail.ToResponse());
    }

    public async Task<ServiceResult<IReadOnlyCollection<OrderDetailResponse>>> GetByOrderIdAsync(int orderId)
    {
        var details = await repository.GetByOrderIdAsync(orderId);
        return details.Count == 0
            ? ServiceResult<IReadOnlyCollection<OrderDetailResponse>>.Fail(StatusCodes.Status404NotFound, $"No OrderDetails found for Order ID {orderId}.")
            : ServiceResult<IReadOnlyCollection<OrderDetailResponse>>.Ok(details.Select(d => d.ToResponse()).ToList());
    }

    public async Task<ServiceResult<OrderDetailResponse>> CreateAsync(OrderDetailUpsertRequest request)
    {
        if (await repository.GetOrderAsync(request.OrderId) is null)
        {
            return ServiceResult<OrderDetailResponse>.Fail(StatusCodes.Status404NotFound, "Order not found.");
        }
        var course = await repository.GetCourseAsync(request.CourseId);
        if (course is null)
        {
            return ServiceResult<OrderDetailResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.");
        }

        var detail = new OrderDetail
        {
            OrderId = request.OrderId,
            CourseId = request.CourseId,
            Quantity = request.Quantity,
            Price = course.Price,
            Amount = course.Price * request.Quantity,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = "system"
        };

        await repository.AddAsync(detail);
        await repository.SaveChangesAsync();
        var created = await repository.GetByIdAsync(detail.Id);
        return ServiceResult<OrderDetailResponse>.Created(created!.ToResponse(), "Order detail created successfully.");
    }

    public async Task<ServiceResult<OrderDetailResponse>> UpdateAsync(int id, OrderDetailUpsertRequest request)
    {
        var detail = await repository.GetByIdAsync(id);
        if (detail is null)
        {
            return ServiceResult<OrderDetailResponse>.Fail(StatusCodes.Status404NotFound, "Order detail not found.");
        }
        var course = await repository.GetCourseAsync(request.CourseId);
        if (course is null)
        {
            return ServiceResult<OrderDetailResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.");
        }

        detail.OrderId = request.OrderId;
        detail.CourseId = request.CourseId;
        detail.Quantity = request.Quantity;
        detail.Price = course.Price;
        detail.Amount = course.Price * request.Quantity;
        detail.UpdatedAt = DateTime.UtcNow;

        await repository.SaveChangesAsync();
        var updated = await repository.GetByIdAsync(id);
        return ServiceResult<OrderDetailResponse>.Ok(updated!.ToResponse(), "Order detail updated successfully.");
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id)
    {
        var detail = await repository.GetByIdAsync(id);
        if (detail is null)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status404NotFound, "Order detail not found.");
        }

        repository.Remove(detail);
        await repository.SaveChangesAsync();
        return ServiceResult<object>.Ok(null, "Order detail deleted successfully.");
    }
}
