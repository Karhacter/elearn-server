using System.ComponentModel.DataAnnotations;

namespace elearn_server.Domain.Entities;

public class Topic : BaseEntity
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public bool IsDeleted { get; set; } = false;

    // Navigation property
    public ICollection<Post> Posts { get; set; } = new List<Post>();
}
