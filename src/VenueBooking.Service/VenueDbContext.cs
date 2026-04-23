using Microsoft.EntityFrameworkCore;
using VenueBooking.Service.Models;

namespace VenueBooking.Service;

public class VenueDbContext : DbContext
{
    public VenueDbContext(DbContextOptions<VenueDbContext> options)
        : base(options)
    {
    }

    public DbSet<Venue> Venues { get; set; }
    public DbSet<VenueImage> VenueImages { get; set; }
}