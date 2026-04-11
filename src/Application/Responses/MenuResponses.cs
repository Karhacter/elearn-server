namespace elearn_server.Application.Responses;

public class MenuResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; }
    public int? ParentId { get; set; }
    public int? Order { get; set; }
    public bool IsDeleted { get; set; }
    public List<MenuResponse> SubMenus { get; set; } = new();
}
