public class CreateBookingRequest
{
    public Guid VenueId { get; set; }
    public Guid UserId { get; set; }

    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
}