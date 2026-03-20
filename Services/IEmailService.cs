namespace elearn_server.Services;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string toEmail, string fullName, string resetLink);
    bool IsConfigured();
}
