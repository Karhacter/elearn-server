using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace elearn_server.Application.DTOs.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class StrongPasswordAttribute : ValidationAttribute
{
    private const int MinimumPasswordLength = 6;
    private static readonly Regex PasswordRegex = new(
        @"^(?=.*[a-z])(?=.*\d)(?=.*[^A-Za-z0-9])[A-Z].*$",
        RegexOptions.Compiled
    );

    public StrongPasswordAttribute()
    {
        ErrorMessage = $"Password must be at least {MinimumPasswordLength} characters long, start with an uppercase letter, contain at least one lowercase letter, one number, and one special character.";
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return true;
        }

        if (value is not string password)
        {
            return false;
        }

        return password.Length >= MinimumPasswordLength && PasswordRegex.IsMatch(password);
    }
}
