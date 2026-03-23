namespace elearn_server.Application.Interfaces;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string toEmail, string fullName, string resetLink);
    Task SendEmailVerificationAsync(string toEmail, string fullName, string verificationLink);
    bool IsConfigured();
}
