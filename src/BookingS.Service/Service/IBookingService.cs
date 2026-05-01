using BookingS.Service.Model;
using Booking = BookingS.Service.Model.Booking;

namespace BookingS.Service.Application.Services;

public interface IBookingService
{
    Task<Booking> CreateBookingAsync(CreateBookingRequest request, Guid userId, string userEmail);
    Task<List<Booking>> GetMyBookingsAsync(Guid userId);
    Task<List<Booking>?> GetVenueBookingsForOwnerAsync(Guid venueId, Guid ownerId);
    Task<List<BookedRangeResponse>> GetBookedRangesAsync(Guid venueId);
    Task<CancelBookingResult> CancelAsync(Guid id, Guid userId, string? userEmail);
}
