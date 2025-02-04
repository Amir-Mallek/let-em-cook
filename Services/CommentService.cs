using let_em_cook.DTO;
using let_em_cook.Models;
using let_em_cook.Repositories;
using let_em_cook.Services.ServiceContracts;

namespace let_em_cook.Services;

public class CommentService : ICommentService
{
    private readonly CommentRepository _commentRepository;
    
    public CommentService(CommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }
    
    public async Task<Comment> CreateComment(CommentDto commentDto, string userId)
    {
        var comment = new Comment
        {
            Content = commentDto.Content,
            ReviewId = commentDto.ReviewId,
            ParentCommentId = commentDto.ParentCommentId,
            UserId = userId,
            CreatedAt = DateTime.Now
        };
        
        await _commentRepository.AddAsync(comment);
        await _commentRepository.SaveChangesAsync();
        
        return comment;
    }
    
    public async Task<ICollection<Comment>> GetReviewComments(
        int reviewId,
        int page,
        int pageSize,
        string sortBy,
        string sortOrder)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        
        return await _commentRepository.GetReviewComments(reviewId, page, pageSize, sortBy, sortOrder);
    }
    
    public async Task<ICollection<Comment>> GetCommentReplies(
        int commentId,
        int page,
        int pageSize,
        string sortBy,
        string sortOrder)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        
        return await _commentRepository.GetCommentReplies(commentId, page, pageSize, sortBy, sortOrder);
    }
    
    public async Task<Comment> GetComment(int commentId)
    {
        var comment = await _commentRepository.GetByIdAsync(commentId);
        
        if (comment == null)
        {
            throw new KeyNotFoundException("Comment not found.");
        }
        
        return comment;
    }
    
    public async Task<Comment> UpdateComment(Comment comment, string userId)
    {
        var originalComment = await _commentRepository.GetByIdAsync(comment.CommentId);

        if (originalComment == null) throw new KeyNotFoundException("Comment not found");
        if (originalComment.UserId != userId || originalComment.CommentId != comment.CommentId)
        {
            throw new UnauthorizedAccessException("You are not authorized to update this comment");
        }

        _commentRepository.Update(comment);
        await _commentRepository.SaveChangesAsync();

        return originalComment;
    }
    
    public async Task DeleteComment(int commentId, string userId)
    {
        var comment = await _commentRepository.GetByIdAsync(commentId);
        
        if (comment == null)
        {
            throw new KeyNotFoundException("Comment not found.");
        }
        
        if (comment.UserId != userId)
        {
            throw new UnauthorizedAccessException("You are not authorized to delete this comment.");
        }
        
        _commentRepository.Delete(comment);
        await _commentRepository.SaveChangesAsync();
    }
}