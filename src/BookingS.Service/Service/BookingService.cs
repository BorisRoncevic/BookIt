using BookingS.Service.Application.Interfaces;
using BookingS.Service.Model;
using Booking = BookingS.Service.Model.Booking;

namespace BookingS.Service.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _repo;
    private readonly INotificationClient _notificationClient;
    private readonly IVenueClient _venueClient;
    private readonly ILogger<BookingService> _logger;

    public BookingService(
        IBookingRepository repo,
        INotificationClient notificationClient,
        IVenueClient venueClient,
        ILogger<BookingService> logger)
    {
        _repo = repo;
        _notificationClient = notificationClient;
        _venueClient = venueClient;
        _logger = logger;
    }

    public async Task<List<BookedRangeResponse>> GetBookedRangesAsync(Guid venueId)
    {
        return await _repo.GetBookedRangesAsync(venueId);
    }

    public async Task<List<Booking>> GetMyBookingsAsync(Guid userId)
    {
        return await _repo.GetByUserIdAsync(userId);
    }

    public async Task<List<Booking>?> GetVenueBookingsForOwnerAsync(Guid venueId, Guid ownerId)
    {
        var venueOwnerId = await _venueClient.GetVenueOwnerIdAsync(venueId);

        if (venueOwnerId != ownerId)
            return null;

        return await _repo.GetByVenueIdAsync(venueId);
    }

    public async Task<Booking> CreateBookingAsync(CreateBookingRequest request, Guid userId, string userEmail)
    {
        if (request.CheckIn >= request.CheckOut)
            throw new Exception("Invalid date range");

        bool overlap = await _repo.HasOverlapAsync(
            request.VenueId,
            request.CheckIn,
            request.CheckOut
        );

        if (overlap)
            throw new Exception("Dates not available");

        int nights = (request.CheckOut - request.CheckIn).Days;
        decimal pricePerNight = 100;

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            VenueId = request.VenueId,
            UserId = userId,
            UserEmail = userEmail,
            CheckIn = request.CheckIn,
            CheckOut = request.CheckOut,
            TotalPrice = nights * pricePerNight,
            Status = BookingStatus.Confirmed,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(booking);

        try
        {
            await _notificationClient.SendBookingCreatedAsync(new BookingCreatedEvent
            {
                BookingId = booking.Id,
                Email = userEmail,
                VenueId = booking.VenueId,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                TotalPrice = booking.TotalPrice
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send booking confirmation email for booking {BookingId}", booking.Id);
        }

        return booking;
    }

    public async Task<CancelBookingResult> CancelAsync(Guid id, Guid userId, string? userEmail)
    {
        var booking = await _repo.GetByIdAsync(id);

        if (booking == null)
            return CancelBookingResult.NotFound;

        if (booking.Status == BookingStatus.Cancelled)
            return CancelBookingResult.AlreadyCancelled;

        var venueOwnerId = await _venueClient.GetVenueOwnerIdAsync(booking.VenueId);
        var canCancel = booking.UserId == userId || venueOwnerId == userId;

        if (!canCancel)
            return CancelBookingResult.Forbidden;

        if (booking.CheckOut.Date < DateTime.UtcNow.Date)
            return CancelBookingResult.PastBooking;

        booking.Status = BookingStatus.Cancelled;

        await _repo.UpdateAsync(booking);

        var notificationEmail = booking.UserEmail;
        if (string.IsNullOrWhiteSpace(notificationEmail) && booking.UserId == userId)
            notificationEmail = userEmail;

        if (!string.IsNullOrWhiteSpace(notificationEmail))
        {
            try
            {
                await _notificationClient.SendBookingCancelledAsync(new BookingCancelledEvent
                {
                    BookingId = booking.Id,
                    Email = notificationEmail,
                    VenueId = booking.VenueId,
                    CheckIn = booking.CheckIn,
                    CheckOut = booking.CheckOut,
                    TotalPrice = booking.TotalPrice
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send booking cancellation email for booking {BookingId}", booking.Id);
            }
        }

        return CancelBookingResult.Cancelled;
    }
}
