using Microsoft.EntityFrameworkCore;

namespace BookingS.Service;
using BookingS.Service.Model;
public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options)
        : base(options) {}

    public DbSet<Booking> Bookings { get; set; }
}