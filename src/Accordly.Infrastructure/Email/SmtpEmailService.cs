using Accordly.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Accordly.Infrastructure.Email;

/// <summary>SMTP email adapter for local and hosted environments.</summary>
public sealed class SmtpEmailService(IConfiguration configuration) : IEmailService
{
    /// <inheritdoc />
    public async Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        using var client = new SmtpClient(configuration["Email:Host"] ?? "localhost", int.TryParse(configuration["Email:Port"], out var port) ? port : 1025);
        using var message = new MailMessage("no-reply@accordly.local", to, subject, body);
        await client.SendMailAsync(message, cancellationToken);
    }
}
