namespace Accordly.Application.Common.Interfaces;

/// <summary>Sends transactional email.</summary>
public interface IEmailService
{
    /// <summary>Sends a plain-text email.</summary>
    Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
}
