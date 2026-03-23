
using Microsoft.AspNetCore.Mvc;
using elearn_server.Models;
using elearn_server.Data;

[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly AppDbContext _context;

    public CommentController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAllComments() => Ok(_context.Comments.ToList());

    [HttpPost]
    public IActionResult AddComment(Comment comment)
    {
        _context.Comments.Add(comment);
        _context.SaveChanges();
        return Ok(comment);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteComment(int id)
    {
        var comment = _context.Comments.Find(id);
        if (comment == null) return NotFound();
        _context.Comments.Remove(comment);
        _context.SaveChanges();
        return NoContent();
    }
}
