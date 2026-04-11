using System.ComponentModel.DataAnnotations;

namespace elearn_server.Domain.Entities;

public class Menu : BaseEntity
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Url { get; set; }

    public int? ParentId { get; set; }

    public int? Order { get; set; }

    public bool IsDeleted { get; set; } = false;

    // Hierarchy navigation
    public Menu? ParentMenu { get; set; }
    public ICollection<Menu> SubMenus { get; set; } = new List<Menu>();
}
