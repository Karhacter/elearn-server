using elearn_server.Application.Requests;
using elearn_server.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace elearn_server.Presentation.Controllers;

[Route("api/[controller]")]
[Authorize(Roles = "Student")]
public class WishlistController(IWishlistService wishlistService) : ApiControllerBase
{
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetWishlist(int userId) => FromResult(await wishlistService.GetByUserIdAsync(userId));

    [HttpPost]
    public async Task<IActionResult> AddToWishlist([FromBody] WishlistCreateRequest request) => FromResult(await wishlistService.CreateAsync(request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveFromWishlist(int id) => FromResult(await wishlistService.DeleteAsync(id));
}
