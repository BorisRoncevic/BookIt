using Notification.Service;

public interface INotificationService
{
    Task SendBookingCreatedAsync(BookingCreatedEvent e);
}

public class NotificationService : INotificationService
{
    private readonly IEmailService _email;

    public NotificationService(IEmailService email)
    {
        _email = email;
    }

    public async Task SendBookingCreatedAsync(BookingCreatedEvent e)
    {
        var subject = "Booking confirmed";

        var body = $@"
            <h2>Booking confirmed</h2>
            <p>Your booking is successful.</p>
            <p><b>From:</b> {e.CheckIn:dd.MM.yyyy}</p>
            <p><b>To:</b> {e.CheckOut:dd.MM.yyyy}</p>
        ";

        await _email.SendAsync(e.Email, subject, body);
    }
}