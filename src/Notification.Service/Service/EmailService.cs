namespace Notification.Service;

public class EmailService : IEmailService
{
    public Task SendAsync(string to, string subject, string body)
    {
        return Task.CompletedTask;
    }
}
