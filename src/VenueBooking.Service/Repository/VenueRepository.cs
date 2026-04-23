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
            .ToListAsync();
    }

    public async Task<Venue?> GetByIdAsync(Guid id)
    {
        return await _context.Venues
            .Include(v => v.Images)
            .Include(v => v.Amenities)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task UpdateAsync(Venue venue)
    {
        _context.Venues.Update(venue);
        await _context.SaveChangesAsync();
    }
}