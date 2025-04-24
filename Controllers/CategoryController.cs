using Microsoft.AspNetCore.Mvc;
using elearn_server.Data;
using elearn_server.Models;

namespace elearn_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/category
        [HttpGet]
        public IActionResult GetCategories()
        {
            var categories = _context.Categories.ToList();
            return Ok(categories);
        }

        // GET: api/category/{id}
        [HttpGet("detail/{id}")]
        public IActionResult GetCategoryById(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
                return NotFound();
            return Ok(category);
        }

        // POST: api/category
        [HttpPost("add")]
        public IActionResult CreateCategory(Category category)
        {
            category.CreatedAt = DateTime.Now;
            category.UpdatedBy = "system";
            category.UpdatedAt = DateTime.Now;
            var existing = _context.Categories.SingleOrDefault(u => u.Name == category.Name);
            if (existing != null)
            {
                return Conflict(new { message = "Tên Thể Loai Đã Tồn Tại." });
            }
            if (category == null)
                return BadRequest();

            _context.Categories.Add(category);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
        }

        // PUT: api/category/{id}
        [HttpPut("edit/{id}")]
        public IActionResult UpdateCategory(int id, Category category)
        {
            var existingCourse = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (existingCourse == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }

            existingCourse.Name = category.Name;
            existingCourse.Description = category.Description;

            _context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/category/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
                return NotFound();

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
