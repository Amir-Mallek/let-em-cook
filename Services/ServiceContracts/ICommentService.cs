using let_em_cook.DTO;
using let_em_cook.Models;

namespace let_em_cook.Services.ServiceContracts;

public interface ICommentService
{
    public Task<Comment> CreateComment(CommentDto comment, string userId);
    
    public Task<ICollection<Comment>> GetReviewComments(
        int reviewId,
        int page,
        int pageSize,
        string sortBy,
        string sortOrder);
    
    public Task<ICollection<Comment>> GetCommentReplies(
        int commentId,
        int page,
        int pageSize,
        string sortBy,
        string sortOrder);
    
    public Task<Comment> GetComment(int commentId);
    
    public Task<Comment> UpdateComment(Comment comment, string userId);
    
    public Task DeleteComment(int commentId, string userId);
}