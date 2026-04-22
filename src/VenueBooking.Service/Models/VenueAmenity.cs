namespace VenueBooking.Service.Models;
public class VenueAmenity{
    public Guid VenueId { get; set; }
    public int AmenityId { get; set; }

    public Venue? Venue { get; set; }
    public Amenity? Amenity { get; set; }
}
