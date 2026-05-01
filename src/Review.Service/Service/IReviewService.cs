using Review.Service.Model;

namespace Review.Service.Service;

public interface IReviewService
{
    Task<(CreateReviewResult Result, Review.Service.Model.Review? Review)> CreateAsync(
        CreateReviewRequest request,
        Guid userId);

    Task<List<Review.Service.Model.Review>> GetByVenueIdAsync(Guid venueId);
    Task<ReviewSummaryResponse> GetSummaryAsync(Guid venueId);
}
