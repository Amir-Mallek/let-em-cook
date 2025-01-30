using let_em_cook.DTO;
using let_em_cook.Models;

namespace let_em_cook.Services.ServiceContracts;

public interface IReviewService
{
    Task<Review> CreateReview(ReviewDto reviewDto, string userId);

    Task<ICollection<Review>> GetReviews(
        int recipeId,
        int page,
        int pageSize,
        string sortBy,
        string sortOrder);

    Task<Review> GetReview(int reviewId);

    Task<Review?> UpdateReview(Review review, string userId);

    Task DeleteReview(int reviewId, string userId);
}