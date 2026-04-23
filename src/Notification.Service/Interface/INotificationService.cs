
namespace Notification.Service;
public class EmailService : IEmailService
{
    public async Task SendAsync(string to, string subject, string body)
    {
        // SMTP logic (MailKit)
    }
}