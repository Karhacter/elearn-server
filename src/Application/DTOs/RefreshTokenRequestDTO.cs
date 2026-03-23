using System.ComponentModel.DataAnnotations;

namespace elearn_server.Application.DTOs;

public class RefreshTokenRequestDTO
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
