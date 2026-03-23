using System.Net;
using System.Net.Mail;
using elearn_server.Application.Interfaces;

namespace elearn_server.Infrastructure.Services;

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

    public async Task SendEmailVerificationAsync(string toEmail, string fullName, string verificationLink)
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
            Subject = "Verify your email",
            Body =
                $"Hello {fullName},\n\n" +
                "Welcome to E-Learn.\n" +
                $"Verify your email here: {verificationLink}\n\n" +
                "If you did not create this account, you can ignore this email.",
            IsBodyHtml = false
        };

        message.To.Add(toEmail);

        await client.SendMailAsync(message);
    }
}
