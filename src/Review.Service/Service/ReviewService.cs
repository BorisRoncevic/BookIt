using Review.Service.Model;
using Review.Service.Repository;

namespace Review.Service.Service;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _repo;

    public ReviewService(IReviewRepository repo)
    {
        _repo = repo;
    }

    public async Task<(CreateReviewResult Result, Review.Service.Model.Review? Review)> CreateAsync(
        CreateReviewRequest request,
        Guid userId)
    {
        if (request.Rating < 1 || request.Rating > 5)
            return (CreateReviewResult.InvalidRating, null);

        var comment = request.Comment.Trim();
        if (string.IsNullOrWhiteSpace(comment))
            return (CreateReviewResult.EmptyComment, null);

        if (await _repo.ExistsForUserAsync(request.VenueId, userId))
            return (CreateReviewResult.Duplicate, null);

        var review = new Review.Service.Model.Review
        {
            Id = Guid.NewGuid(),
            VenueId = request.VenueId,
            UserId = userId,
            Rating = request.Rating,
            Comment = comment,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(review);

        return (CreateReviewResult.Created, review);
    }

    public async Task<List<Review.Service.Model.Review>> GetByVenueIdAsync(Guid venueId)
    {
        return await _repo.GetByVenueIdAsync(venueId);
    }

    public async Task<ReviewSummaryResponse> GetSummaryAsync(Guid venueId)
    {
        return await _repo.GetSummaryAsync(venueId);
    }
}
