using System.ComponentModel.DataAnnotations;

namespace elearn_server.Application.DTOs;

public class VerifyEmailRequestDTO
{
    [Required]
    public string Token { get; set; } = string.Empty;
}
