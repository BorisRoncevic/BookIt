namespace Notification.Service;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(string to, string subject, string body)
    {
        _logger.LogWarning(
            "SendGrid is not configured. Email to {Email} with subject '{Subject}' was not sent.",
            to,
            subject);

        throw new InvalidOperationException(
            "SendGrid is not configured. Set SENDGRID_API_KEY and SENDGRID_SENDER_EMAIL.");
    }
}
