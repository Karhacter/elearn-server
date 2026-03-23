using System.ComponentModel.DataAnnotations;

namespace elearn_server.Application.DTOs
{
    public class UserLoginDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [MaxLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MaxLength(255, ErrorMessage = "Password cannot exceed 255 characters")]
        [RegularExpression(@".*\S.*", ErrorMessage = "Password cannot be blank")]
        public string Password { get; set; } = string.Empty;
    }
}
