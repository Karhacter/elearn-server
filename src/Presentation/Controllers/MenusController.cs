using elearn_server.Application.Interfaces;
using elearn_server.Application.Requests;
using Microsoft.AspNetCore.Mvc;

namespace elearn_server.Presentation.Controllers;

[Route("api/menus")]
public class MenusController : ApiControllerBase
{
    private readonly IMenuService _menuService;

    public MenusController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllMenus()
    {
        return FromResult(await _menuService.GetAllMenusAsync());
    }

    [HttpGet("tree")]
    public async Task<IActionResult> GetMenuTree()
    {
        return FromResult(await _menuService.GetMenuTreeAsync());
    }

    [HttpPost]
    public async Task<IActionResult> CreateMenu([FromBody] CreateMenuRequest request)
    {
        return FromResult(await _menuService.CreateMenuAsync(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMenu(int id, [FromBody] UpdateMenuRequest request)
    {
        return FromResult(await _menuService.UpdateMenuAsync(id, request));
    }

    [HttpPatch("{id}/toggle-soft-delete")]
    public async Task<IActionResult> ToggleSoftDelete(int id)
    {
        return FromResult(await _menuService.ToggleSoftDeleteAsync(id));
    }
}
