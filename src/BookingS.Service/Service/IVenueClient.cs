namespace BookingS.Service.Application.Services;

public interface IVenueClient
{
    Task<Guid?> GetVenueOwnerIdAsync(Guid venueId);
}
