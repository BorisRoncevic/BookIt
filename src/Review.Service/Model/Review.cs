namespace Review.Service.Model;

public class Review
{
    public Guid Id { get; set; }
    public Guid VenueId { get; set; }
    public Guid UserId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
