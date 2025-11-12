using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WorldCup.Shared.Services;

/// <summary>
/// Azure Communication Services email sender implementation
/// </summary>
public class AzureEmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AzureEmailSender> _logger;

    public AzureEmailSender(IConfiguration configuration, ILogger<AzureEmailSender> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        var connectionString = _configuration["Email:Azure:ConnectionString"];
        var fromEmail = _configuration["Email:Azure:FromEmail"];

        if (string.IsNullOrEmpty(connectionString))
        {
            _logger.LogWarning("Azure Communication Services connection string is missing. Email not sent to {Email}", toEmail);
            return;
        }

        if (string.IsNullOrEmpty(fromEmail))
        {
            _logger.LogWarning("Azure Communication Services sender email is missing. Email not sent to {Email}", toEmail);
            return;
        }

        try
        {
            var emailClient = new EmailClient(connectionString);

            var emailMessage = new EmailMessage(
                senderAddress: fromEmail,
                content: new EmailContent(subject)
                {
                    Html = htmlBody
                },
                recipients: new EmailRecipients(new List<EmailAddress>
                {
                    new EmailAddress(toEmail)
                }));

            EmailSendOperation emailSendOperation = await emailClient.SendAsync(
                WaitUntil.Completed,
                emailMessage);

            _logger.LogInformation("Email sent successfully to {Email}. Status: {Status}", 
                toEmail, emailSendOperation.Value.Status);
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Azure Communication Services request failed while sending email to {Email}. Error Code: {ErrorCode}", 
                toEmail, ex.ErrorCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email} using Azure Communication Services", toEmail);
            throw;
        }
    }
}
