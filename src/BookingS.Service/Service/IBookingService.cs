using BookingS.Service.Model;
using Booking = BookingS.Service.Model.Booking;

namespace BookingS.Service.Application.Services;

public interface IBookingService
{
    Task<Booking> CreateBookingAsync(CreateBookingRequest request, Guid userId);
    Task<bool> CancelAsync(Guid id);
}
