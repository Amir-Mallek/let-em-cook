using System.Security.Claims;
using let_em_cook.DTO;
using let_em_cook.Models;
using let_em_cook.Services.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace let_em_cook.Controllers;

public class CommentController : Controller
{
    private readonly ICommentService _commentService;
    
    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }
    
    [HttpPost]
    [Authorize]
    public async Task<Comment> CreateComment(CommentDto commentDto)
    {
        if (commentDto.ReviewId == null && commentDto.ParentCommentId == null)
        {
            throw new ArgumentException("ReviewId or ParentCommentId must be provided.");
        }
        
        var userId = User.Claims.First(c => c.Type == ClaimTypes.Name).Value;
        return await _commentService.CreateComment(commentDto, userId);
    }
    
    
    [HttpGet("review")]
    public async Task<ICollection<Comment>> GetReviewComments(
        int reviewId,
        int page = 1,
        int pageSize = 10,
        string sortBy = "CreatedAt",
        string sortOrder = "desc")
    {
        return await _commentService.GetReviewComments(reviewId, page, pageSize, sortBy, sortOrder);
    }
    
    [HttpGet("reply")]
    public async Task<ICollection<Comment>> GetCommentReplies(
        int commentId,
        int page = 1,
        int pageSize = 10,
        string sortBy = "CreatedAt",
        string sortOrder = "desc")
    {
        return await _commentService.GetCommentReplies(commentId, page, pageSize, sortBy, sortOrder);
    }
    
    [HttpGet("{commentId:int}")]
    public async Task<Comment> GetComment(int commentId)
    {
        return await _commentService.GetComment(commentId);
    }
    
    [HttpPut]
    [Authorize]
    public async Task<Comment> UpdateComment(Comment comment)
    {
        var userId = User.Claims.First(c => c.Type == ClaimTypes.Name).Value;
        return await _commentService.UpdateComment(comment, userId);
    }
    
    [HttpDelete("{commentId:int}")]
    [Authorize]
    public async Task DeleteComment(int commentId)
    {
        var userId = User.Claims.First(c => c.Type == ClaimTypes.Name).Value;
        await _commentService.DeleteComment(commentId, userId);
    }
}