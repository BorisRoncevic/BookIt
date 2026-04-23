
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string htmlBody);
}

public class EmailService : IEmailService
{
    private readonly EmailSettings _cfg;

    public EmailService(IOptions<EmailSettings> options)
    {
        _cfg = options.Value;
    }

    public async Task SendAsync(string to, string subject, string htmlBody)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(_cfg.SenderName, _cfg.SenderEmail));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        message.Body = new TextPart("html")
        {
            Text = htmlBody
        };

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(_cfg.SmtpServer, _cfg.Port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_cfg.SenderEmail, _cfg.Password);

        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }
}