namespace WorldCup.Shared.Services;

/// <summary>
/// Service for sending emails to users
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Sends an email asynchronously
    /// </summary>
    /// <param name="toEmail">Recipient email address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="htmlBody">Email body in HTML format</param>
    Task SendEmailAsync(string toEmail, string subject, string htmlBody);
}
