using System.ComponentModel.DataAnnotations;

namespace elearn_server.Application.Requests;

public class CreateContactRequest
{
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Message { get; set; } = string.Empty;
}

public class UpdateContactRequest
{
    public string Status { get; set; } = string.Empty;
}
