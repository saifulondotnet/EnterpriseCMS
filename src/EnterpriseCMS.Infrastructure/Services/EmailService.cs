using EnterpriseCMS.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace EnterpriseCMS.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
    {
        try
        {
            var host = _config["Email:SmtpHost"] ?? "localhost";
            var port = int.Parse(_config["Email:SmtpPort"] ?? "25");
            var from = _config["Email:From"] ?? "noreply@enterprisecms.com";

            using var client = new SmtpClient(host, port);
            var user = _config["Email:Username"];
            var pass = _config["Email:Password"];
            if (!string.IsNullOrEmpty(user))
                client.Credentials = new NetworkCredential(user, pass);
            client.EnableSsl = bool.Parse(_config["Email:EnableSsl"] ?? "false");

            var msg = new MailMessage(from, to, subject, htmlBody) { IsBodyHtml = true };
            await client.SendMailAsync(msg, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
        }
    }
}
