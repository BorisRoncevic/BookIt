using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Review.Service.Model;
using Review.Service.Service;

namespace Review.Service.Controller;

[ApiController]
[Route("api/reviews")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _service;

    public ReviewController(IReviewService service)
    {
        _service = service;
    }

    [HttpGet("venue/{venueId:guid}")]
    public async Task<ActionResult<List<ReviewResponse>>> GetVenueReviews(Guid venueId)
    {
        var reviews = await _service.GetByVenueIdAsync(venueId);
        return Ok(reviews.Select(ToResponse));
    }

    [HttpGet("venue/{venueId:guid}/summary")]
    public async Task<ActionResult<ReviewSummaryResponse>> GetVenueSummary(Guid venueId)
    {
        return Ok(await _service.GetSummaryAsync(venueId));
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ReviewResponse>> Create([FromBody] CreateReviewRequest request)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var (result, review) = await _service.CreateAsync(request, userId);

        return result switch
        {
            CreateReviewResult.Created => CreatedAtAction(
                nameof(GetVenueReviews),
                new { venueId = review!.VenueId },
                ToResponse(review)),
            CreateReviewResult.Duplicate => Conflict("You have already reviewed this venue."),
            CreateReviewResult.InvalidRating => BadRequest("Rating must be between 1 and 5."),
            CreateReviewResult.EmptyComment => BadRequest("Comment is required."),
            _ => BadRequest()
        };
    }

    private static ReviewResponse ToResponse(Review.Service.Model.Review review)
    {
        return new ReviewResponse(
            review.Id,
            review.VenueId,
            review.UserId,
            review.Rating,
            review.Comment,
            review.CreatedAt);
    }

    private bool TryGetUserId(out Guid userId)
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("sub");

        return Guid.TryParse(value, out userId);
    }
}
