using Microsoft.AspNetCore.Mvc;
using elearn_server.Models;
using elearn_server.Data;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentController : ControllerBase
{
    private readonly AppDbContext _context;

    public EnrollmentController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAllEnrollments() => Ok(_context.Enrollments.ToList());

    [HttpPost]
    public IActionResult EnrollUser(Enrollment enrollment)
    {
        _context.Enrollments.Add(enrollment);
        _context.SaveChanges();
        return Ok(enrollment);
    }

    [HttpDelete("{id}")]
    public IActionResult UnenrollUser(int id)
    {
        var enrollment = _context.Enrollments.Find(id);
        if (enrollment == null) return NotFound();
        _context.Enrollments.Remove(enrollment);
        _context.SaveChanges();
        return NoContent();
    }
}
