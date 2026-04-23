namespace BookingS.Service.Model;

public class CreateBookingRequest
{
    public Guid VenueId { get; set; }

    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
}
