using Notification.Service;

namespace Notification.Service;

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
            <p>Your booking has been confirmed.</p>
            <p><b>Booking ID:</b> {e.BookingId}</p>
            <p><b>Venue ID:</b> {e.VenueId}</p>
            <p><b>From:</b> {e.CheckIn:dd.MM.yyyy}</p>
            <p><b>To:</b> {e.CheckOut:dd.MM.yyyy}</p>
            <p><b>Total price:</b> {e.TotalPrice:0.00} EUR</p>
        ";

        await _email.SendAsync(e.Email, subject, body);
    }

    public async Task SendBookingCancelledAsync(BookingCancelledEvent e)
    {
        var subject = "Booking cancelled";

        var body = $@"
            <h2>Booking cancelled</h2>
            <p>Your booking has been cancelled.</p>
            <p><b>Booking ID:</b> {e.BookingId}</p>
            <p><b>Venue ID:</b> {e.VenueId}</p>
            <p><b>From:</b> {e.CheckIn:dd.MM.yyyy}</p>
            <p><b>To:</b> {e.CheckOut:dd.MM.yyyy}</p>
            <p><b>Total price:</b> {e.TotalPrice:0.00} EUR</p>
        ";

        await _email.SendAsync(e.Email, subject, body);
    }
}
