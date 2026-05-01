namespace BookingS.Service.Model;

public class Booking
{
    public Guid Id { get; set; }

    public Guid VenueId { get; set; }
    public Guid UserId { get; set; }
    public string? UserEmail { get; set; }

    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }

    public decimal TotalPrice { get; set; }

    public BookingStatus Status { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
