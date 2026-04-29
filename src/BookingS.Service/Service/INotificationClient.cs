using BookingS.Service.Model;

namespace BookingS.Service.Application.Services;

public interface INotificationClient
{
    Task SendBookingCreatedAsync(BookingCreatedEvent e);
}
