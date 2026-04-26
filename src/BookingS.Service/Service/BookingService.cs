using BookingS.Service.Application.Interfaces;
using BookingS.Service.Model;
using Notification.Service;
using Booking = BookingS.Service.Model.Booking;

namespace BookingS.Service.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _repo;
    private readonly INotificationService _notificationService;

    public BookingService(IBookingRepository repo, INotificationService notificationService)
    {
        _repo = repo;
        _notificationService = notificationService;
    }

    public async Task<List<BookedRangeResponse>> GetBookedRangesAsync(Guid venueId)
{
    return await _repo.GetBookedRangesAsync(venueId);
}

    public async Task<Booking> CreateBookingAsync(CreateBookingRequest request, Guid userId)
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
            CheckIn = request.CheckIn,
            CheckOut = request.CheckOut,
            TotalPrice = nights * pricePerNight,
            Status = BookingStatus.Confirmed,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(booking);

        try
        {
            await _notificationService.SendBookingCreatedAsync(new BookingCreatedEvent
            {
                BookingId = booking.Id,
                Email = "user@mail.com",
                VenueId = booking.VenueId,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                TotalPrice = booking.TotalPrice
            });
        }
        catch
        {
        }

        return booking;
    }

    public async Task<bool> CancelAsync(Guid id)
    {
        var booking = await _repo.GetByIdAsync(id);

        if (booking == null)
            return false;

        booking.Status = BookingStatus.Cancelled;

        await _repo.UpdateAsync(booking);

        return true;
    }
}
