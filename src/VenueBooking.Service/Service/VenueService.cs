using VenueBooking.Service.Application.Interfaces;
using VenueBooking.Service.Models;

namespace VenueBooking.Service.Application.Services;

public class VenueService : IVenueService
{
    private readonly IVenueRepository _repo;

    public VenueService(IVenueRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<Venue>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<Venue?> GetByIdAsync(Guid id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<List<Amenity>> GetAmenitiesAsync()
    {
        return await _repo.GetAmenitiesAsync();
    }

    public async Task<Venue> CreateAsync(Venue venue)
    {
        venue.Id = Guid.NewGuid();
        venue.CreatedAt = DateTime.UtcNow;

        foreach (var amenity in venue.Amenities)
        {
            amenity.VenueId = venue.Id;
        }

        await _repo.AddAsync(venue);

        return venue;
    }

    public async Task<bool> UpdateAsync(Guid id, Venue venue)
    {
        var exists = await _repo.ExistsAsync(id);
        if (!exists)
            return false;

        venue.Id = id;
        foreach (var amenity in venue.Amenities)
        {
            amenity.VenueId = id;
        }

        await _repo.UpdateAsync(venue);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var exists = await _repo.ExistsAsync(id);
        if (!exists)
            return false;

        await _repo.DeleteAsync(id);

        return true;
    }
}
