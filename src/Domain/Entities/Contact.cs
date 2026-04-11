using System.ComponentModel.DataAnnotations;

namespace elearn_server.Domain.Entities;

public class Contact : BaseEntity
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Message { get; set; } = string.Empty;

    public string Status { get; set; } = "Pending";

    public bool IsDeleted { get; set; } = false;
}
