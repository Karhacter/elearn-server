using System.ComponentModel.DataAnnotations;

namespace elearn_server.DTO;

public class ForgotPasswordRequestDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
