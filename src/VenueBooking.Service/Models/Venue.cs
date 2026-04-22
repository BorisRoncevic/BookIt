namespace VenueBooking.Service.Models;
public class Venue
{
    public Guid Id { get; set; }

    public Guid OwnerId { get; set; } 
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    public decimal PricePerNight { get; set; }

    public int MaxGuests { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<VenueImage> Images { get; set; } = new();
    public List<VenueAmenity> Amenities { get; set; } = new();
}

