using elearn_server.Application.Requests;
using elearn_server.Application.Responses;
using elearn_server.Application.DTOs;
using elearn_server.Application.Mappings;
using elearn_server.Domain.Entities;
using elearn_server.Infrastructure.Persistence.Repositories;
using elearn_server.Application.Common;
using elearn_server.Domain.Enums;

using elearn_server.Application.Interfaces;
namespace elearn_server.Infrastructure.Services.Commerce;

public class OrderService(IOrderRepository repository) : IOrderService
{
    public async Task<ServiceResult<IReadOnlyCollection<OrderResponse>>> GetAllAsync() =>
        ServiceResult<IReadOnlyCollection<OrderResponse>>.Ok((await repository.GetAllAsync()).Select(o => o.ToResponse()).ToList());

    public async Task<ServiceResult<OrderResponse>> GetByIdAsync(int id)
    {
        var order = await repository.GetByIdAsync(id);
        return order is null
            ? ServiceResult<OrderResponse>.Fail(StatusCodes.Status404NotFound, "Order not found.")
            : ServiceResult<OrderResponse>.Ok(order.ToResponse());
    }

    public async Task<ServiceResult<OrderResponse>> CreateBasicAsync(Order order)
    {
        order.CreatedAt = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;
        order.UpdatedBy = "system";
        await repository.AddAsync(order);
        await repository.SaveChangesAsync();
        var created = await repository.GetByIdAsync(order.OrderID);
        return ServiceResult<OrderResponse>.Created(created!.ToResponse(), "Order created successfully.");
    }

    public async Task<ServiceResult<OrderResponse>> CreateAsync(OrderRequest request)
    {
        if (request.Items is null || request.Items.Count == 0)
        {
            return ServiceResult<OrderResponse>.Fail(StatusCodes.Status400BadRequest, "Order must contain at least one item.");
        }
        if (!await repository.UserExistsAsync(request.UserId))
        {
            return ServiceResult<OrderResponse>.Fail(StatusCodes.Status404NotFound, "User not found.");
        }

        var order = new Order
        {
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            user_id = request.UserId,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = "system",
            OrderDetails = new List<OrderDetail>()
        };

        foreach (var item in request.Items)
        {
            var course = await repository.GetCourseByIdAsync(item.CourseID);
            if (course is null)
            {
                return ServiceResult<OrderResponse>.Fail(StatusCodes.Status404NotFound, $"Course with ID {item.CourseID} not found.");
            }

            order.OrderDetails.Add(new OrderDetail
            {
                CourseId = item.CourseID,
                Quantity = item.Quantity,
                Price = course.Price,
                Amount = course.Price * item.Quantity
            });
        }

        await repository.AddAsync(order);
        await repository.SaveChangesAsync();
        var created = await repository.GetByIdAsync(order.OrderID);
        return ServiceResult<OrderResponse>.Created(created!.ToResponse(), "Order created successfully.");
    }

    public async Task<ServiceResult<OrderResponse>> UpdateAsync(int id, OrderUpdateDTO request)
    {
        var order = await repository.GetByIdAsync(id);
        if (order is null)
        {
            return ServiceResult<OrderResponse>.Fail(StatusCodes.Status404NotFound, "Không tìm thấy đơn hàng.");
        }

        order.Name = request.Name;
        order.Status = request.Status;
        order.UpdatedAt = DateTime.UtcNow;
        order.UpdatedBy = "admin";

        if (request.Items is { Count: > 0 })
        {
            order.OrderDetails.Clear();
            foreach (var item in request.Items)
            {
                var course = await repository.GetCourseByIdAsync(item.CourseID);
                if (course is null)
                {
                    return ServiceResult<OrderResponse>.Fail(StatusCodes.Status400BadRequest, $"Không tìm thấy sản phẩm với id {item.CourseID}");
                }

                order.OrderDetails.Add(new OrderDetail
                {
                    CourseId = course.CourseId,
                    Quantity = item.Quantity,
                    Price = course.Price,
                    Amount = course.Price * item.Quantity
                });
            }
        }

        await repository.SaveChangesAsync();
        var updated = await repository.GetByIdAsync(id);
        return ServiceResult<OrderResponse>.Ok(updated!.ToResponse(), "Order updated successfully.");
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id)
    {
        var order = await repository.GetByIdAsync(id);
        if (order is null)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status404NotFound, "Order not found.");
        }

        repository.Remove(order);
        await repository.SaveChangesAsync();
        return ServiceResult<object>.Ok(null, "Order deleted successfully.");
    }

    public async Task<ServiceResult<IReadOnlyCollection<OrderResponse>>> GetByUserIdAsync(int userId)
    {
        if (!await repository.UserExistsAsync(userId))
        {
            return ServiceResult<IReadOnlyCollection<OrderResponse>>.Fail(StatusCodes.Status404NotFound, "User chưa đăng nhập");
        }

        var orders = await repository.GetByUserIdAsync(userId);
        return orders.Count == 0
            ? ServiceResult<IReadOnlyCollection<OrderResponse>>.Fail(StatusCodes.Status404NotFound, "No orders found for this user.")
            : ServiceResult<IReadOnlyCollection<OrderResponse>>.Ok(orders.Select(o => o.ToResponse()).ToList());
    }
}
