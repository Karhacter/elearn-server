using elearn_server.Application.Common;
using elearn_server.Application.Interfaces;
using elearn_server.Application.Mappings;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;
using elearn_server.Domain.Entities;
using elearn_server.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace elearn_server.Infrastructure.Services;

public class MenuService : IMenuService
{
    private readonly AppDbContext _context;

    public MenuService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<List<MenuResponse>>> GetAllMenusAsync()
    {
        var menus = await _context.Set<Menu>()
            .Where(m => !m.IsDeleted)
            .ToListAsync();
        return ServiceResult<List<MenuResponse>>.Ok(menus.Select(m => m.ToResponse()).ToList());
    }

    public async Task<ServiceResult<List<MenuResponse>>> GetMenuTreeAsync()
    {
        var menus = await _context.Set<Menu>()
            .Where(m => !m.IsDeleted)
            .ToListAsync();

        var menuDict = menus.ToDictionary(m => m.Id);
        var rootMenus = new List<Menu>();

        foreach (var menu in menus)
        {
            if (menu.ParentId.HasValue && menuDict.TryGetValue(menu.ParentId.Value, out var parent))
            {
                if (parent.SubMenus == null) parent.SubMenus = new List<Menu>();
                parent.SubMenus.Add(menu);
            }
            else
            {
                rootMenus.Add(menu);
            }
        }

        return ServiceResult<List<MenuResponse>>.Ok(rootMenus.Select(m => m.ToResponse()).ToList());
    }

    public async Task<ServiceResult<MenuResponse>> GetMenuByIdAsync(int id)
    {
        var menu = await _context.Set<Menu>().FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);
        if (menu == null) return ServiceResult<MenuResponse>.Fail(404, "Menu not found.");
        return ServiceResult<MenuResponse>.Ok(menu.ToResponse());
    }

    public async Task<ServiceResult<MenuResponse>> CreateMenuAsync(CreateMenuRequest request)
    {
        var menu = new Menu
        {
            Name = request.Name,
            Url = request.Url,
            ParentId = request.ParentId,
            Order = request.Order,
            CreatedAt = DateTime.UtcNow
        };
        _context.Set<Menu>().Add(menu);
        await _context.SaveChangesAsync();
        return ServiceResult<MenuResponse>.Created(menu.ToResponse());
    }

    public async Task<ServiceResult<MenuResponse>> UpdateMenuAsync(int id, UpdateMenuRequest request)
    {
        var menu = await _context.Set<Menu>().FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);
        if (menu == null) return ServiceResult<MenuResponse>.Fail(404, "Menu not found.");

        menu.Name = request.Name;
        menu.Url = request.Url;
        menu.ParentId = request.ParentId;
        menu.Order = request.Order;
        menu.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return ServiceResult<MenuResponse>.Ok(menu.ToResponse());
    }

    public async Task<ServiceResult<bool>> ToggleSoftDeleteAsync(int id)
    {
        var menu = await _context.Set<Menu>().FirstOrDefaultAsync(m => m.Id == id);
        if (menu == null) return ServiceResult<bool>.Fail(404, "Menu not found.");

        menu.IsDeleted = !menu.IsDeleted;
        menu.DeletedAt = menu.IsDeleted ? DateTime.UtcNow : null;
        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true, "Menu soft delete toggled.");
    }
}
