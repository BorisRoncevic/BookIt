using Microsoft.EntityFrameworkCore;
using Review.Service.Model;

namespace Review.Service;

public class ReviewDbContext : DbContext
{
    public ReviewDbContext(DbContextOptions<ReviewDbContext> options)
        : base(options)
    {
    }

    public DbSet<Review.Service.Model.Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Review.Service.Model.Review>()
            .HasIndex(review => new { review.VenueId, review.UserId })
            .IsUnique();

        modelBuilder.Entity<Review.Service.Model.Review>()
            .Property(review => review.Comment)
            .HasMaxLength(1000);
    }
}
