public class BookingService
{
    private readonly IBookingRepository _repo;

    public BookingService(IBookingRepository repo)
    {
        _repo = repo;
    }

    public async Task<Booking> CreateBookingAsync(CreateBookingRequest request)
    {
        if (request.CheckIn >= request.CheckOut)
            throw new Exception("Invalid date range");

        var existing = await _repo.GetByVenueIdAsync(request.VenueId);

        bool overlap = existing.Any(b =>
            b.Status != BookingStatus.Cancelled &&
            request.CheckIn < b.CheckOut &&
            request.CheckOut > b.CheckIn
        );

        if (overlap)
            throw new Exception("Dates not available");

        int nights = (request.CheckOut - request.CheckIn).Days;
        decimal pricePerNight = 100; 

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            VenueId = request.VenueId,
            UserId = request.UserId,
            CheckIn = request.CheckIn,
            CheckOut = request.CheckOut,
            TotalPrice = nights * pricePerNight,
            Status = BookingStatus.Confirmed
        };

        await _repo.AddAsync(booking);

        return booking;
    }

    public async Task CancelAsync(Guid id)
    {
        var booking = await _repo.GetByIdAsync(id);
        if (booking == null) throw new Exception("Not found");

        booking.Status = BookingStatus.Cancelled;

        await _repo.UpdateAsync(booking);
    }
}