namespace elearn_server.Common.Options;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpireDays { get; set; } = 7;
    public int RefreshTokenExpireDays { get; set; } = 30;
}

public class AuthSecurityOptions
{
    public const string SectionName = "AuthSecurity";

    public int EmailVerificationExpireHours { get; set; } = 24;
    public bool RequireEmailVerification { get; set; } = true;
}
