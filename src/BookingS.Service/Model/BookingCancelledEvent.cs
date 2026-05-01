namespace BookingS.Service.Model;

public class BookingCancelledEvent
{
    public Guid BookingId { get; set; }
    public string Email { get; set; } = null!;
    public Guid VenueId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public decimal TotalPrice { get; set; }
}
