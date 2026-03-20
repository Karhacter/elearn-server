using System.Net;
using System.Net.Mail;

namespace elearn_server.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool IsConfigured()
    {
        return !string.IsNullOrWhiteSpace(_configuration["Smtp:Host"]) &&
               !string.IsNullOrWhiteSpace(_configuration["Smtp:Port"]) &&
               !string.IsNullOrWhiteSpace(_configuration["Smtp:Username"]) &&
               !string.IsNullOrWhiteSpace(_configuration["Smtp:Password"]) &&
               !string.IsNullOrWhiteSpace(_configuration["Smtp:FromEmail"]);
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string fullName, string resetLink)
    {
        if (!IsConfigured())
        {
            throw new InvalidOperationException("SMTP settings are not configured.");
        }

        var host = _configuration["Smtp:Host"]!;
        var port = int.Parse(_configuration["Smtp:Port"]!);
        var username = _configuration["Smtp:Username"]!;
        var password = _configuration["Smtp:Password"]!;
        var fromEmail = _configuration["Smtp:FromEmail"]!;
        var fromName = _configuration["Smtp:FromName"] ?? "E-Learn";
        var enableSsl = bool.TryParse(_configuration["Smtp:EnableSsl"], out var parsedEnableSsl) && parsedEnableSsl;

        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(username, password),
            EnableSsl = enableSsl
        };

        using var message = new MailMessage
        {
            From = new MailAddress(fromEmail, fromName),
            Subject = "Password reset request",
            Body =
                $"Hello {fullName},\n\n" +
                "We received a request to reset your password.\n" +
                $"Reset your password here: {resetLink}\n\n" +
                "If you did not request this, you can ignore this email.\n" +
                "This link will expire in 15 minutes.",
            IsBodyHtml = false
        };

        message.To.Add(toEmail);

        await client.SendMailAsync(message);
    }
}
