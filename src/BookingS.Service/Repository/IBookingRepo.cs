using BookingS.Service.Model;
using Booking = BookingS.Service.Model.Booking;

namespace BookingS.Service.Application.Interfaces;

public interface IBookingRepository
{
    Task AddAsync(Booking booking);
    Task<Booking?> GetByIdAsync(Guid id);
    Task UpdateAsync(Booking booking);

    Task<List<Booking>> GetByVenueIdAsync(Guid venueId);

    Task<bool> HasOverlapAsync(Guid venueId, DateTime checkIn, DateTime checkOut);
}
