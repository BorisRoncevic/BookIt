namespace VenueBooking.Service.Application.Interfaces;
using VenueBooking.Service.Models;

public interface IVenueRepository
{
    Task<Venue?> GetByIdAsync(Guid id);
    Task<List<Venue>> GetAllAsync();
    Task<List<Amenity>> GetAmenitiesAsync();

    Task AddAsync(Venue venue);
    Task UpdateAsync(Venue venue);
    Task DeleteAsync(Guid id);

    Task<bool> ExistsAsync(Guid id);
}
