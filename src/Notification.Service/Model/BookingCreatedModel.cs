public class BookingCreatedEvent
{
    public Guid BookingId { get; set; }
    public string Email { get; set; } = null!;
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
}