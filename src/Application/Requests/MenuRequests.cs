using System.ComponentModel.DataAnnotations;

namespace elearn_server.Application.Requests;

public class CreateMenuRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; }
    public int? ParentId { get; set; }
    public int? Order { get; set; }
}

public class UpdateMenuRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; }
    public int? ParentId { get; set; }
    public int? Order { get; set; }
}
