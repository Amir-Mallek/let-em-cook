using let_em_cook.DTO;
using let_em_cook.Models;
using let_em_cook.Repositories;
using let_em_cook.Services.ServiceContracts;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;

namespace let_em_cook.Services;

public class ReviewService : IReviewService
{
    private readonly ReviewRepository _reviewRepository;
    
    public ReviewService(ReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }
    
    public async Task<Review> CreateReview(ReviewDto reviewDto, string userId)
    {
        var review = new Review
        {
            Content = reviewDto.Content,
            Rating = reviewDto.Rating,
            RecipeId = reviewDto.RecipeId,
            UserId = userId,
            CreatedAt = DateTime.Now
        };
        
        return await _reviewRepository.CreateReview(review);
    }
    
    public async Task<ICollection<Review>> GetReviews(
        int recipeId,
        int page, 
        int pageSize, 
        string sortBy, 
        string sortOrder)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        
        return await _reviewRepository.GetReviews(recipeId, page, pageSize, sortBy, sortOrder);
    }
    
    public async Task<Review> GetReview(int reviewId)
    {
        var review = await _reviewRepository.GetReview(reviewId);
        
        if (review == null)
        {
            throw new KeyNotFoundException("Review not found.");
        }
        
        return review;
    }
    
    public async Task<Review?> UpdateReview(Review review, string userId)
    {
        if (review.UserId != userId)
        {
            throw new UnauthorizedAccessException("You are not authorized to update this review.");
        }
        
        return await _reviewRepository.UpdateReview(review, userId);
    }
    
    public async Task DeleteReview(int reviewId, string userId)
    {
        await _reviewRepository.DeleteReview(reviewId, userId);
    }
}