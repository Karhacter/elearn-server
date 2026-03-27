using elearn_server.Application.Requests;
using elearn_server.Application.DTOs;
using elearn_server.Domain.Entities;
using elearn_server.Infrastructure.Persistence.Repositories;
using elearn_server.Application.Common;
using elearn_server.Application.Interfaces;
namespace elearn_server.Infrastructure.Services.Commerce;

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
