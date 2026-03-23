using System.ComponentModel.DataAnnotations;
using elearn_server.Application.DTOs.Validation;

namespace elearn_server.Application.DTOs;

public class ResetPasswordRequestDTO
{
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    [StrongPassword]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
