namespace Review.Service.Model;

public record CreateReviewRequest
{
    public Guid VenueId { get; init; }
    public int Rating { get; init; }
    public string Comment { get; init; } = string.Empty;
}

public record ReviewResponse(
    Guid Id,
    Guid VenueId,
    Guid UserId,
    int Rating,
    string Comment,
    DateTime CreatedAt
);

public record ReviewSummaryResponse(
    Guid VenueId,
    double AverageRating,
    int ReviewCount
);
