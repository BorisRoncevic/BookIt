using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Notification.Service.Services;

public class SendGridEmailService : IEmailService
{
    private readonly SendGridSettings _settings;
    private readonly SendGridClient _client;

    public SendGridEmailService(IOptions<SendGridSettings> options)
    {
        _settings = options.Value;
        _client = new SendGridClient(_settings.ApiKey);
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        var from = new EmailAddress(_settings.SenderEmail, _settings.SenderName);
        var toEmail = new EmailAddress(to);

        var msg = MailHelper.CreateSingleEmail(
            from,
            toEmail,
            subject,
            plainTextContent: StripHtml(body),
            htmlContent: body
        );

        var response = await _client.SendEmailAsync(msg);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Body.ReadAsStringAsync();
            throw new Exception($"SendGrid failed: {error}");
        }
    }

    // ✔️ fallback plain text (spam filter + compatibility)
    private static string StripHtml(string html)
    {
        return System.Text.RegularExpressions.Regex
            .Replace(html, "<.*?>", string.Empty);
    }
}