using BookingS.Service.Application.Interfaces;
using BookingS.Service.Model;
using Microsoft.EntityFrameworkCore;
using Booking = BookingS.Service.Model.Booking;

namespace BookingS.Service.Infrastructure;

public class BookingRepository : IBookingRepository
{
    private readonly BookingDbContext _context;

    public BookingRepository(BookingDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Booking booking)
    {
        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();
    }

    public async Task<Booking?> GetByIdAsync(Guid id)
    {
        return await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task UpdateAsync(Booking booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Booking>> GetByVenueIdAsync(Guid venueId)
    {
        return await _context.Bookings
            .AsNoTracking()
            .Where(b => b.VenueId == venueId)
            .OrderByDescending(b => b.CheckIn)
            .ToListAsync();
    }

    public async Task<bool> HasOverlapAsync(Guid venueId, DateTime checkIn, DateTime checkOut)
    {
        return await _context.Bookings.AnyAsync(b =>
            b.VenueId == venueId &&
            b.Status == BookingStatus.Confirmed &&
            checkIn < b.CheckOut &&
            checkOut > b.CheckIn
        );
    }
    public async Task<List<BookedRangeResponse>> GetBookedRangesAsync(Guid venueId)
{
    return await _context.Bookings
        .Where(b =>
            b.VenueId == venueId &&
            b.Status == BookingStatus.Confirmed
        )
        .Select(b => new BookedRangeResponse
        {
            CheckIn = b.CheckIn,
            CheckOut = b.CheckOut
        })
        .ToListAsync();
}

    
    public async Task<List<Booking>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Bookings
            .AsNoTracking()
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }
}
