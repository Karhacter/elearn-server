using Microsoft.AspNetCore.Mvc;
using elearn_server.Models;
using elearn_server.Data;

[ApiController]
[Route("api/[controller]")]
public class WishlistController : ControllerBase
{
    private readonly AppDbContext _context;

    public WishlistController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{userId}")]
    public IActionResult GetWishlist(int userId)
    {
        var wishlist = _context.Wishlists.Where(w => w.UserId == userId).ToList();
        return Ok(wishlist);
    }

    [HttpPost]
    public IActionResult AddToWishlist(Wishlist wishlist)
    {
        _context.Wishlists.Add(wishlist);
        _context.SaveChanges();
        return Ok(wishlist);
    }

    [HttpDelete("{id}")]
    public IActionResult RemoveFromWishlist(int id)
    {
        var item = _context.Wishlists.Find(id);
        if (item == null) return NotFound();
        _context.Wishlists.Remove(item);
        _context.SaveChanges();
        return NoContent();
    }
}
