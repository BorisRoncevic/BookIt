
namespace Venue.Service.Models;
public class VenueImage
{
    public Guid Id { get; set; }
    public Guid VenueId { get; set; }

    public string Url { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }

    public Venue? Venue { get; set; }
}