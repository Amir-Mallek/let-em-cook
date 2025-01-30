using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using let_em_cook.DTO;
using let_em_cook.Models;
using let_em_cook.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace let_em_cook.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController : Controller
{
    private readonly ReviewService _reviewService;
    
    public ReviewController(ReviewService reviewService)
    {
        _reviewService = reviewService;
    }
    
    [HttpPost]
    [Authorize]
    public async Task<Review> CreateReview(ReviewDto reviewDto)
    {
        var userId = User.Claims.First(c => c.Type == ClaimTypes.Name).Value;
        return await _reviewService.CreateReview(reviewDto, userId);
    }
    
    [HttpGet]
    public async Task<ICollection<Review>> GetReviews(
        [FromQuery][Required] int recipeId, 
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = "created_at",
        [FromQuery] string sortOrder = "desc")
    {
        return await _reviewService.GetReviews(recipeId, page, pageSize, sortBy, sortOrder);
    }
    
    [HttpGet("{reviewId:int}")]
    public async Task<Review?> GetReview(int reviewId)
    {
        return await _reviewService.GetReview(reviewId);
    }
    
    [HttpPut]
    [Authorize]
    public async Task<Review?> UpdateReview(Review review)
    {
        var userId = User.Claims.First(c => c.Type == "Name").Value;
        return await _reviewService.UpdateReview(review, userId);
    }
    
    [HttpDelete("{reviewId:int}")]
    [Authorize]
    public async Task DeleteReview(int reviewId)
    {
        var userId = User.Claims.First(c => c.Type == "Name").Value;
        await _reviewService.DeleteReview(reviewId, userId);
    }
}