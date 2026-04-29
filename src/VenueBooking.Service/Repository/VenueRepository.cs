using Microsoft.EntityFrameworkCore;
using VenueBooking.Service.Application.Interfaces;
using VenueBooking.Service.Models;

namespace VenueBooking.Service;

public class VenueRepository : IVenueRepository
{
    private readonly VenueDbContext _context;

    public VenueRepository(VenueDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Venue venue)
    {
        await _context.Venues.AddAsync(venue);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var venue = await _context.Venues.FindAsync(id);

        if (venue != null)
        {
            _context.Venues.Remove(venue);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Venues.AnyAsync(v => v.Id == id);
    }

    public async Task<List<Venue>> GetAllAsync()
    {
        return await _context.Venues
            .Include(v => v.Images)
            .Include(v => v.Amenities)
            .ThenInclude(va => va.Amenity)
            .ToListAsync();
    }

    public async Task<Venue?> GetByIdAsync(Guid id)
    {
        return await _context.Venues
            .Include(v => v.Images)
            .Include(v => v.Amenities)
            .ThenInclude(va => va.Amenity)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<List<Amenity>> GetAmenitiesAsync()
    {
        return await _context.Amenities
            .OrderBy(a => a.Id)
            .ToListAsync();
    }

    public async Task UpdateAsync(Venue venue)
    {
        var existingVenue = await _context.Venues
            .Include(v => v.Amenities)
            .FirstOrDefaultAsync(v => v.Id == venue.Id);

        if (existingVenue == null)
            return;

        existingVenue.Title = venue.Title;
        existingVenue.Description = venue.Description;
        existingVenue.City = venue.City;
        existingVenue.Country = venue.Country;
        existingVenue.Address = venue.Address;
        existingVenue.PricePerNight = venue.PricePerNight;
        existingVenue.MaxGuests = venue.MaxGuests;
        existingVenue.Bedrooms = venue.Bedrooms;
        existingVenue.Bathrooms = venue.Bathrooms;

        existingVenue.Amenities.Clear();
        foreach (var amenity in venue.Amenities)
        {
            existingVenue.Amenities.Add(new VenueAmenity
            {
                VenueId = existingVenue.Id,
                AmenityId = amenity.AmenityId
            });
        }

        await _context.SaveChangesAsync();
    }
}
