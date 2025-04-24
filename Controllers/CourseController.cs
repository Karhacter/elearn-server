using Microsoft.AspNetCore.Mvc;
using elearn_server.Models;
using elearn_server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class CourseController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public CourseController(AppDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [HttpGet]
    public IActionResult GetAllCourses() => Ok(_context.Courses.ToList());

    // New paging endpoint for courses
    [HttpGet("paged")]
    public IActionResult GetPagedCourses(int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var totalCourses = _context.Courses.Count();
        var totalPages = (int)Math.Ceiling(totalCourses / (double)pageSize);

        var courses = _context.Courses
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var response = new
        {
            TotalCount = totalCourses,
            PageSize = pageSize,
            CurrentPage = pageNumber,
            TotalPages = totalPages,
            Courses = courses
        };

        return Ok(response);
    }

    [HttpGet("detail/{id}")]
    public IActionResult GetCourseById(int id)
    {
        var course = _context.Courses.Find(id);
        if (course == null) return NotFound();
        return Ok(course);
    }

    [HttpPost("add")]
    public IActionResult CreateCourse(Course course)
    {
        course.CreatedAt = DateTime.Now;
        course.UpdatedBy = "system";
        course.UpdatedAt = DateTime.Now;

        var existingTitle = _context.Courses.SingleOrDefault(u => u.Title == course.Title);
        if (existingTitle != null)
        {
            return Conflict(new { message = "Tên Khóa Học Đã Tồn Tại." });
        }

        if (course.InstructorId == 0)
        {
            // course.InstructorId = GetCurrentUserId();
        }

        _context.Courses.Add(course);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetCourseById), new { id = course.CourseId }, course);
    }


    [HttpPut("edit/{id}")]
    public IActionResult UpdateCourse(int id, Course course)
    {

        var existingCourse = _context.Courses.FirstOrDefault(c => c.CourseId == id);

        if (existingCourse == null)
        {
            return NotFound($"Course with ID {id} not found.");
        }

        // Update only the necessary properties
        existingCourse.Title = course.Title;
        existingCourse.Description = course.Description;
        existingCourse.Duration = course.Duration;
        existingCourse.Thumbnail = course.Thumbnail;
        existingCourse.Price = course.Price;
        existingCourse.Discount = course.Discount;
        existingCourse.GenreId = course.GenreId;
        existingCourse.InstructorId = course.InstructorId;
        // Update more properties as needed

        // Save changes
        _context.SaveChanges();

        return NoContent(); // Returns HTTP 204 (No Content) - successful update without returning any data.
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


    [HttpGet("category/{categoryId}")]
    public IActionResult GetCoursesByCategoryId(int categoryId)
    {
        var courses = _context.Courses
            .Include(c => c.Genre)
            .Include(c => c.Instructor)
            .Where(c => c.GenreId == categoryId)
            .ToList();

        if (courses == null || courses.Count == 0)
        {
            return NotFound($"No courses found for category ID {categoryId}.");
        }

        return Ok(courses);
    }

    [HttpPatch("{id}/image")]
    public IActionResult UpdateCourseImage(int id, [FromBody] string imageUrl)
    {
        var course = _context.Courses.Find(id);
        if (course == null)
        {
            return NotFound($"Course with ID {id} not found.");
        }

        course.Image = imageUrl;
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPost("{id}/upload-image")]
    public async Task<IActionResult> UploadCourseImage(int id, IFormFile imageFile)
    {
        var course = _context.Courses.Find(id);
        if (course == null)
        {
            return NotFound($"Course with ID {id} not found.");
        }

        if (imageFile == null || imageFile.Length == 0)
        {
            return BadRequest("No image file provided.");
        }

        var webRootPath = _environment.WebRootPath;
        if (string.IsNullOrEmpty(webRootPath))
        {
            webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        }

        var uploadsFolder = Path.Combine(webRootPath, "uploads");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        course.Image = $"/uploads/{fileName}";
        _context.SaveChanges();

        return Ok(new { ImageUrl = course.Image });
    }
}
