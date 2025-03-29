using Microsoft.AspNetCore.Mvc;
using elearn_server.Models;
using elearn_server.Data;

[ApiController]
[Route("api/[controller]")]
public class CourseController : ControllerBase
{
    private readonly AppDbContext _context;

    public CourseController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAllCourses() => Ok(_context.Courses.ToList());

    [HttpGet("{id}")]
    public IActionResult GetCourseById(int id)
    {
        var course = _context.Courses.Find(id);
        if (course == null) return NotFound();
        return Ok(course);
    }

    [HttpPost]
    public IActionResult CreateCourse(Course course)
    {
        _context.Courses.Add(course);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetCourseById), new { id = course.CourseId }, course);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateCourse(int id, Course course)
    {
        if (id != course.CourseId) return BadRequest();
        _context.Entry(course).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        _context.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteCourse(int id)
    {
        var course = _context.Courses.Find(id);
        if (course == null) return NotFound();
        _context.Courses.Remove(course);
        _context.SaveChanges();
        return NoContent();
    }
}
