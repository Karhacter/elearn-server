namespace elearn_server.Application.Responses;

public class AuthenticatedUserResponse
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
    public string? Gender { get; set; }
    public DateTime? Birthday { get; set; }
    public string? CountryCode { get; set; }
    public string? CountryName { get; set; }
    public string? CityCode { get; set; }
    public string? CityName { get; set; }
    public string? Street { get; set; }
}

public class LoginResponse
{
    public AuthenticatedUserResponse User { get; set; } = new();
    public DateTime ExpiresAtUtc { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

public class AuthCheckResponse
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
