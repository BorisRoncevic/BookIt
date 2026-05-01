namespace Notification.Service;

public interface INotificationService
{
    Task SendBookingCreatedAsync(BookingCreatedEvent e);
    Task SendBookingCancelledAsync(BookingCancelledEvent e);
}
