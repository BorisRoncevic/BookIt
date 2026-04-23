using BookingS.Service.Application.Interfaces;
using BookingS.Service.Model;
using Booking = BookingS.Service.Model.Booking;

namespace BookingS.Service.Application.Services;

public class BookingService
{
    private readonly IBookingRepository _repo;

    public BookingService(IBookingRepository repo)
    {
        _repo = repo;
    }

    public async Task<Booking> CreateBookingAsync(CreateBookingRequest request, Guid userId)
    {
        // ✔️ validacija
        if (request.CheckIn >= request.CheckOut)
            throw new Exception("Invalid date range");

        // ✔️ overlap check direktno u DB
        bool overlap = await _repo.HasOverlapAsync(
            request.VenueId,
            request.CheckIn,
            request.CheckOut
        );

        if (overlap)
            throw new Exception("Dates not available");

        int nights = (request.CheckOut - request.CheckIn).Days;

        // TODO: kasnije iz VenueService
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
