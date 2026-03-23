using elearn_server.Application.Requests;
using elearn_server.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace elearn_server.Presentation.Controllers;

[Route("api/[controller]")]
public class CategoryController(ICategoryService categoryService) : ApiControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategories() => FromResult(await categoryService.GetAllAsync());

    [HttpGet("detail/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategoryById(int id) => FromResult(await categoryService.GetByIdAsync(id));

    [HttpPost("add")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryUpsertRequest request) => FromResult(await categoryService.CreateAsync(request));

    [HttpPut("edit/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpsertRequest request) => FromResult(await categoryService.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCategory(int id) => FromResult(await categoryService.DeleteAsync(id));
}
