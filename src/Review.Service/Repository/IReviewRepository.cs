using Review.Service.Model;

namespace Review.Service.Repository;

public interface IReviewRepository
{
    Task AddAsync(Review.Service.Model.Review review);
    Task<bool> ExistsForUserAsync(Guid venueId, Guid userId);
    Task<List<Review.Service.Model.Review>> GetByVenueIdAsync(Guid venueId);
    Task<ReviewSummaryResponse> GetSummaryAsync(Guid venueId);
}
