using Microsoft.EntityFrameworkCore;
using Review.Service.Model;

namespace Review.Service.Repository;

public class ReviewRepository : IReviewRepository
{
    private readonly ReviewDbContext _context;

    public ReviewRepository(ReviewDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Review.Service.Model.Review review)
    {
        await _context.Reviews.AddAsync(review);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsForUserAsync(Guid venueId, Guid userId)
    {
        return await _context.Reviews.AnyAsync(review =>
            review.VenueId == venueId &&
            review.UserId == userId);
    }

    public async Task<List<Review.Service.Model.Review>> GetByVenueIdAsync(Guid venueId)
    {
        return await _context.Reviews
            .AsNoTracking()
            .Where(review => review.VenueId == venueId)
            .OrderByDescending(review => review.CreatedAt)
            .ToListAsync();
    }

    public async Task<ReviewSummaryResponse> GetSummaryAsync(Guid venueId)
    {
        var reviews = _context.Reviews
            .AsNoTracking()
            .Where(review => review.VenueId == venueId);

        var count = await reviews.CountAsync();
        var average = count == 0
            ? 0
            : Math.Round(await reviews.AverageAsync(review => review.Rating), 1);

        return new ReviewSummaryResponse(venueId, average, count);
    }
}
