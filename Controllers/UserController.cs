using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using elearn_server.Models;
using elearn_server.Data;
using System.Collections.Generic;
using elearn_server.DTO;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public UserController(AppDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [HttpGet]
    public IActionResult GetAllUsers()
    {
        var users = _context.Users.ToList();
        return Ok(users);
    }

    [HttpGet("detail/{id}")]
    public IActionResult GetUserById(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpPost("add")]
    public IActionResult CreateUser(UserCreateDTO userDto)
    {
        var user = new User
        {
            FullName = userDto.FullName,
            Email = userDto.Email,
            PhoneNumber = userDto.PhoneNumber,
            Password = userDto.Password,
            Role = userDto.Role,
            ProfilePicture = userDto.ProfilePicture ?? string.Empty,
        };

        _context.Users.Add(user);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, user);
    }

    [HttpPut("edit/{id}")]
    public IActionResult UpdateUser(int id, User user)
    {
        var updateuser = _context.Users.FirstOrDefault(u => u.UserId == id);
        if (updateuser == null)
        {
            return NotFound($"USer with ID {id} not found");
        }

        updateuser.FullName = user.FullName;
        updateuser.Email = user.Email;
        updateuser.PhoneNumber = user.PhoneNumber;
        updateuser.Email = user.Email;
        updateuser.Role = user.Role;
        updateuser.ProfilePicture = user.ProfilePicture ?? string.Empty;

        _context.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null) return NotFound();
        _context.Users.Remove(user);
        _context.SaveChanges();
        return NoContent();
    }

    [HttpPatch("{id}/image")]
    public IActionResult UpdateUserImage(int id, [FromBody] string imageUrl)
    {
        var user = _context.Users.Find(id);
        if (user == null)
        {
            return NotFound($"User with ID {id} not found.");
        }

        user.ProfilePicture = imageUrl;
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPost("{id}/upload-image")]
    public async Task<IActionResult> UploadUserImage(int id, IFormFile imageFile)
    {
        var user = _context.Users.Find(id);
        if (user == null)
        {
            return NotFound($"User with ID {id} not found.");
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

        user.ProfilePicture = $"/uploads/{fileName}";
        _context.SaveChanges();

        return Ok(new { ImageUrl = user.ProfilePicture });
    }
}
