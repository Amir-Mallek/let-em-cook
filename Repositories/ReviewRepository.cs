using let_em_cook.Data;
using let_em_cook.Models;
using Microsoft.EntityFrameworkCore;

namespace let_em_cook.Repositories;

public class ReviewRepository
{
    private readonly ApplicationdbContext _db;
    private readonly DbSet<Review> _dbSet;
    
    public ReviewRepository(ApplicationdbContext db)
    {
        _db = db;
        _dbSet = db.Set<Review>();
    }
    
    public async Task<Review> CreateReview(Review review)
    {
        await _dbSet.AddAsync(review);
        await _db.SaveChangesAsync();
        return review;
    }
    
    public async Task<ICollection<Review>> GetReviews(
        int recipeId,
        int page, 
        int pageSize, 
        string sortBy, 
        string sortOrder)
    {
        var recipeReviews =
            from review in _dbSet
            where review.RecipeId == recipeId
            select review;
        
        var orderBy = sortBy + '_' + sortOrder;

        recipeReviews = orderBy switch
        {
            "created_at_asc" => recipeReviews.OrderBy(r => r.CreatedAt),
            "rating_desc" => recipeReviews.OrderByDescending(r => r.Rating),
            "rating_asc" => recipeReviews.OrderBy(r => r.Rating),
            "popularity_desc" => recipeReviews.OrderByDescending(r => r.Comments.Count()),
            "popularity_asc" => recipeReviews.OrderBy(r => r.Comments.Count()),
            _ => recipeReviews.OrderByDescending(r => r.CreatedAt)
        };
        
        return await recipeReviews
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    
    public async Task<Review?> GetReview(int reviewId)
    {
        return await _dbSet.FindAsync(reviewId);
    }
    
    public async Task<Review?> UpdateReview(Review review, string userId)
    {
        var originalReview = await GetReview(review.ReviewId);

        if (originalReview == null) throw new BadHttpRequestException("Review not found");
        if (originalReview.UserId != userId)
        {
            throw new UnauthorizedAccessException("You are not authorized to update this review");
        }
        
        _dbSet.Update(review);
        await _db.SaveChangesAsync();
        return review;
    }
    
    public async Task DeleteReview(int reviewId, string userId)
    {
        var review = await GetReview(reviewId);
        if (review != null && review.UserId != userId)
        {
            throw new UnauthorizedAccessException("You are not authorized to delete this review");
        }
        
        if (review != null)
        {
            _dbSet.Remove(review);
            await _db.SaveChangesAsync();
        }
    }
}