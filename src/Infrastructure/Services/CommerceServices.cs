using elearn_server.Application.Requests;
using elearn_server.Application.Responses;
using elearn_server.Application.DTOs;
using elearn_server.Application.Mappings;
using elearn_server.Domain.Entities;
using elearn_server.Infrastructure.Persistence.Repositories;
using elearn_server.Application.Common;

namespace elearn_server.Application.Interfaces;

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
            StatusOrderId = request.StatusOrderId,
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

        var status = await repository.GetStatusByIdAsync(request.StatusOrderId);
        if (status is null)
        {
            return ServiceResult<OrderResponse>.Fail(StatusCodes.Status400BadRequest, $"Không tìm thấy trạng thái đơn hàng với id {request.StatusOrderId}");
        }

        order.Name = request.Name;
        order.StatusOrderId = request.StatusOrderId;
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

public class CartService(ICartRepository repository) : ICartService
{
    public async Task<ServiceResult<CartDTO>> GetCartByUserAsync(int userId, bool autoCreate)
    {
        if (await repository.GetUserAsync(userId) is null)
        {
            return ServiceResult<CartDTO>.Fail(StatusCodes.Status404NotFound, "User không tồn tại");
        }

        var cart = await repository.GetByUserIdAsync(userId);
        if (cart is null && autoCreate)
        {
            await repository.AddAsync(new Cart { UserId = userId, CartItems = new List<CartItem>() });
            await repository.SaveChangesAsync();
            cart = await repository.GetByUserIdAsync(userId);
        }

        return cart is null
            ? ServiceResult<CartDTO>.Fail(StatusCodes.Status404NotFound, "Không tìm thấy giỏ hàng cho user này")
            : ServiceResult<CartDTO>.Ok(ToCartDto(cart));
    }

    public async Task<ServiceResult<object>> InitCartsAsync()
    {
        var users = await repository.GetUsersWithoutCartsAsync();
        foreach (var user in users)
        {
            await repository.AddAsync(new Cart { UserId = user.UserId, CartItems = new List<CartItem>() });
        }
        await repository.SaveChangesAsync();
        return ServiceResult<object>.Ok(null, "Đã tạo cart cho tất cả user chưa có.");
    }

    public async Task<ServiceResult<CartDTO>> AddToCartAsync(AddToCartRequest request)
    {
        if (await repository.GetUserAsync(request.UserId) is null)
        {
            return ServiceResult<CartDTO>.Fail(StatusCodes.Status404NotFound, "User không tồn tại");
        }
        var course = await repository.GetCourseAsync(request.CourseID);
        if (course is null)
        {
            return ServiceResult<CartDTO>.Fail(StatusCodes.Status404NotFound, "Sản phẩm không tồn tại");
        }

        var cart = await repository.GetByUserIdAsync(request.UserId);
        if (cart is null)
        {
            await repository.AddAsync(new Cart { UserId = request.UserId, CartItems = new List<CartItem>() });
            await repository.SaveChangesAsync();
            cart = await repository.GetByUserIdAsync(request.UserId);
        }

        var item = cart!.CartItems.FirstOrDefault(ci => ci.CourseID == request.CourseID);
        if (item is null)
        {
            await repository.AddCartItemAsync(new CartItem
            {
                CartId = cart.CartId,
                CourseID = request.CourseID,
                Quantity = request.Quantity,
                PriceAtTime = course.Price
            });
        }
        else
        {
            item.Quantity += request.Quantity;
        }

        await repository.SaveChangesAsync();
        return ServiceResult<CartDTO>.Ok(ToCartDto((await repository.GetByUserIdAsync(request.UserId))!), "Đã thêm sản phẩm vào giỏ hàng");
    }

    public async Task<ServiceResult<CartDTO>> UpdateCartItemAsync(UpdateCartItemRequest request)
    {
        var cart = await repository.GetByUserIdAsync(request.UserId);
        if (cart is null)
        {
            return ServiceResult<CartDTO>.Fail(StatusCodes.Status404NotFound, "Giỏ hàng không tồn tại");
        }

        var item = cart.CartItems.FirstOrDefault(ci => ci.CourseID == request.CourseID);
        if (item is null)
        {
            return ServiceResult<CartDTO>.Fail(StatusCodes.Status404NotFound, "Sản phẩm không có trong giỏ hàng");
        }

        if (request.Quantity <= 0)
        {
            repository.RemoveCartItems([item]);
        }
        else
        {
            item.Quantity = request.Quantity;
        }

        await repository.SaveChangesAsync();
        return ServiceResult<CartDTO>.Ok(ToCartDto((await repository.GetByUserIdAsync(request.UserId))!), "Đã cập nhật sản phẩm trong giỏ hàng");
    }

    public async Task<ServiceResult<CartDTO>> RemoveItemAsync(RemoveCartItemRequest request)
    {
        var cart = await repository.GetByUserIdAsync(request.UserId);
        if (cart is null)
        {
            return ServiceResult<CartDTO>.Fail(StatusCodes.Status404NotFound, "Giỏ hàng không tồn tại");
        }

        var item = cart.CartItems.FirstOrDefault(ci => ci.CourseID == request.CourseID);
        if (item is null)
        {
            return ServiceResult<CartDTO>.Fail(StatusCodes.Status404NotFound, "Sản phẩm không có trong giỏ hàng");
        }

        repository.RemoveCartItems([item]);
        await repository.SaveChangesAsync();
        return ServiceResult<CartDTO>.Ok(ToCartDto((await repository.GetByUserIdAsync(request.UserId))!), "Đã xóa sản phẩm khỏi giỏ hàng");
    }

    public async Task<ServiceResult<CartDTO>> ClearCartAsync(int userId)
    {
        var cart = await repository.GetByUserIdAsync(userId);
        if (cart is null)
        {
            return ServiceResult<CartDTO>.Fail(StatusCodes.Status404NotFound, "Giỏ hàng không tồn tại");
        }

        repository.RemoveCartItems(cart.CartItems);
        await repository.SaveChangesAsync();
        return ServiceResult<CartDTO>.Ok(ToCartDto((await repository.GetByUserIdAsync(userId))!), "Giỏ hàng đã được làm trống");
    }

    private static CartDTO ToCartDto(Cart cart) => new()
    {
        Id = cart.CartId,
        UserId = cart.UserId,
        Items = cart.CartItems?.Where(ci => ci.Course is not null).Select(ci => new CartItemDTO
        {
            CourseID = ci.CourseID,
            CourseTitle = ci.Course!.Title,
            Quantity = ci.Quantity,
            Price = ci.Course.Price,
            Discount = ci.Course.Discount,
            Image = ci.Course.Image,
            Subtotal = (double)(ci.Quantity * ci.Course.Price)
        }).ToList() ?? new List<CartItemDTO>(),
        Total = cart.CartItems?.Where(ci => ci.Course is not null).Sum(ci => (double)(ci.Quantity * ci.Course!.Price)) ?? 0
    };
}

public class WishlistService(IWishlistRepository repository) : IWishlistService
{
    public async Task<ServiceResult<IReadOnlyCollection<WishlistResponse>>> GetByUserIdAsync(int userId) =>
        ServiceResult<IReadOnlyCollection<WishlistResponse>>.Ok((await repository.GetByUserIdAsync(userId)).Select(w => w.ToResponse()).ToList());

    public async Task<ServiceResult<WishlistResponse>> CreateAsync(WishlistCreateRequest request)
    {
        if (!await repository.UserExistsAsync(request.UserId))
        {
            return ServiceResult<WishlistResponse>.Fail(StatusCodes.Status404NotFound, "User not found.");
        }
        if (await repository.GetCourseAsync(request.CourseId) is null)
        {
            return ServiceResult<WishlistResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.");
        }
        if (await repository.GetByUserAndCourseAsync(request.UserId, request.CourseId) is not null)
        {
            return ServiceResult<WishlistResponse>.Fail(StatusCodes.Status409Conflict, "Course is already in wishlist.");
        }

        var wishlist = new Wishlist
        {
            UserId = request.UserId,
            CourseId = request.CourseId,
            AddedDate = DateTime.UtcNow
        };

        await repository.AddAsync(wishlist);
        await repository.SaveChangesAsync();
        var created = await repository.GetByIdAsync(wishlist.Id);
        return ServiceResult<WishlistResponse>.Created(created!.ToResponse(), "Wishlist item created successfully.");
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id)
    {
        var wishlist = await repository.GetByIdAsync(id);
        if (wishlist is null)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status404NotFound, "Wishlist item not found.");
        }

        repository.Remove(wishlist);
        await repository.SaveChangesAsync();
        return ServiceResult<object>.Ok(null, "Wishlist item removed successfully.");
    }
}

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
        if (order.StatusOrder?.Name == "Đã thanh toán")
        {
            return ServiceResult<object>.Fail(StatusCodes.Status400BadRequest, "Đơn hàng đã được thanh toán.");
        }
        var paidStatus = await repository.GetStatusByNameAsync("Đã thanh toán");
        if (paidStatus is null)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status400BadRequest, "Không tìm thấy trạng thái thanh toán.");
        }

        order.StatusOrderId = paidStatus.Id;
        await repository.SaveChangesAsync();
        return ServiceResult<object>.Ok(new { orderId = order.OrderID }, "Thanh toán thành công. Đơn hàng đã được hoàn tất.");
    }
}

public class EnrollmentService(IEnrollmentRepository repository) : IEnrollmentService
{
    public async Task<ServiceResult<IReadOnlyCollection<EnrollmentResponse>>> GetAllAsync() =>
        ServiceResult<IReadOnlyCollection<EnrollmentResponse>>.Ok((await repository.GetAllAsync()).Select(e => e.ToResponse()).ToList());

    public async Task<ServiceResult<EnrollmentResponse>> CreateAsync(EnrollmentCreateRequest request)
    {
        if (await repository.GetUserAsync(request.UserId) is null)
        {
            return ServiceResult<EnrollmentResponse>.Fail(StatusCodes.Status404NotFound, "User not found.");
        }
        var course = await repository.GetCourseAsync(request.CourseId);
        if (course is null)
        {
            return ServiceResult<EnrollmentResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.");
        }
        if (await repository.GetByUserAndCourseAsync(request.UserId, request.CourseId) is not null)
        {
            return ServiceResult<EnrollmentResponse>.Fail(StatusCodes.Status409Conflict, "Enrollment already exists.");
        }

        var enrollment = new Enrollment
        {
            UserId = request.UserId,
            CourseId = request.CourseId,
            EnrollmentDate = DateTime.UtcNow,
            IsActive = true,
            Course = course
        };

        await repository.AddAsync(enrollment);
        await repository.SaveChangesAsync();
        return ServiceResult<EnrollmentResponse>.Created(enrollment.ToResponse(), "Enrollment created successfully.");
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id)
    {
        var enrollment = await repository.GetByIdAsync(id);
        if (enrollment is null)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status404NotFound, "Enrollment not found.");
        }

        repository.Remove(enrollment);
        await repository.SaveChangesAsync();
        return ServiceResult<object>.Ok(null, "Enrollment deleted successfully.");
    }
}

public class CommentService(ICommentRepository repository) : ICommentService
{
    public async Task<ServiceResult<IReadOnlyCollection<CommentResponse>>> GetAllAsync() =>
        ServiceResult<IReadOnlyCollection<CommentResponse>>.Ok((await repository.GetAllAsync()).Select(c => c.ToResponse()).ToList());

    public async Task<ServiceResult<CommentResponse>> CreateAsync(CommentCreateRequest request)
    {
        if (await repository.GetUserAsync(request.UserId) is null)
        {
            return ServiceResult<CommentResponse>.Fail(StatusCodes.Status404NotFound, "User not found.");
        }
        if (await repository.GetCourseAsync(request.CourseId) is null)
        {
            return ServiceResult<CommentResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.");
        }

        var comment = new Comment
        {
            UserId = request.UserId,
            CourseId = request.CourseId,
            Content = request.Content.Trim(),
            CommentDate = DateTime.UtcNow
        };

        await repository.AddAsync(comment);
        await repository.SaveChangesAsync();
        return ServiceResult<CommentResponse>.Created(comment.ToResponse(), "Comment created successfully.");
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id)
    {
        var comment = await repository.GetByIdAsync(id);
        if (comment is null)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status404NotFound, "Comment not found.");
        }

        repository.Remove(comment);
        await repository.SaveChangesAsync();
        return ServiceResult<object>.Ok(null, "Comment deleted successfully.");
    }
}

public class CertificateService(ICertificateRepository repository) : ICertificateService
{
    public async Task<ServiceResult<IReadOnlyCollection<CertificateResponse>>> GetAllAsync() =>
        ServiceResult<IReadOnlyCollection<CertificateResponse>>.Ok((await repository.GetAllAsync()).Select(c => c.ToResponse()).ToList());

    public async Task<ServiceResult<CertificateResponse>> CreateAsync(CertificateCreateRequest request)
    {
        if (await repository.GetUserAsync(request.UserId) is null)
        {
            return ServiceResult<CertificateResponse>.Fail(StatusCodes.Status404NotFound, "User not found.");
        }
        if (await repository.GetCourseAsync(request.CourseId) is null)
        {
            return ServiceResult<CertificateResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.");
        }

        var certificate = new Certificate
        {
            UserId = request.UserId,
            CourseId = request.CourseId,
            CertificateUrl = request.CertificateUrl,
            DateIssued = DateTime.UtcNow
        };

        await repository.AddAsync(certificate);
        await repository.SaveChangesAsync();
        return ServiceResult<CertificateResponse>.Created(certificate.ToResponse(), "Certificate created successfully.");
    }
}
