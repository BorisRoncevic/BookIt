namespace Review.Service.Model;

public enum CreateReviewResult
{
    Created,
    Duplicate,
    InvalidRating,
    EmptyComment
}
