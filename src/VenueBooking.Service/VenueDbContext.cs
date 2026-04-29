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
    public DbSet<Amenity> Amenities { get; set; }
    public DbSet<VenueAmenity> VenueAmenities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VenueAmenity>()
            .HasKey(x => new { x.VenueId, x.AmenityId });

        modelBuilder.Entity<VenueAmenity>()
            .HasOne(x => x.Venue)
            .WithMany(x => x.Amenities)
            .HasForeignKey(x => x.VenueId);

        modelBuilder.Entity<VenueAmenity>()
            .HasOne(x => x.Amenity)
            .WithMany()
            .HasForeignKey(x => x.AmenityId);

        modelBuilder.Entity<Venue>()
            .Property(x => x.PricePerNight)
            .HasPrecision(18, 2);
    }
}
