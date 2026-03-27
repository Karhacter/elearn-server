namespace elearn_server.Domain.Entities;

public class BaseEntity
{
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }  // Ensure it's a string for 'system'
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }  // Ensure it's a string for 'system'
    public DateTime? DeletedAt { get; set; } // Solf -delete 
    public string? DeletedBy { get; set; }  // Ensure it's a string for 'system'
}
