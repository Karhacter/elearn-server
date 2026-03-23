using elearn_server.Application.Requests;
using elearn_server.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace elearn_server.Presentation.Controllers;

[Route("api/[controller]")]
[Authorize(Roles = "Student")]
public class CartController(ICartService cartService) : ApiControllerBase
{
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetCartByUser(int userId) => FromResult(await cartService.GetCartByUserAsync(userId, true));

    [HttpPost("init-carts")]
    public async Task<IActionResult> CreateCartsForAllUsers() => FromResult(await cartService.InitCartsAsync());

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetCart(int userId) => FromResult(await cartService.GetCartByUserAsync(userId, false));

    [HttpPost("add-item")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request) => FromResult(await cartService.AddToCartAsync(request));

    [HttpPut("update-item")]
    public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemRequest request) => FromResult(await cartService.UpdateCartItemAsync(request));

    [HttpDelete("remove-item")]
    public async Task<IActionResult> RemoveItem([FromBody] RemoveCartItemRequest request) => FromResult(await cartService.RemoveItemAsync(request));

    [HttpDelete("clear/{userId}")]
    public async Task<IActionResult> ClearCart(int userId) => FromResult(await cartService.ClearCartAsync(userId));
}
