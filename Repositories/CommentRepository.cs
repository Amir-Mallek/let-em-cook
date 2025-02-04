using let_em_cook.Data;
using let_em_cook.Models;
using Microsoft.EntityFrameworkCore;

namespace let_em_cook.Repositories;

public class CommentRepository : Repository<Comment>
{
    public CommentRepository(ApplicationdbContext context) : base(context) {}
    
    public async Task<ICollection<Comment>> GetReviewComments(
        int reviewId,
        int page,
        int pageSize,
        string sortBy,
        string sortOrder)
    {
        var reviewComments =
            from comment in _dbSet
            where comment.ReviewId == reviewId
            select comment;
        
        var orderBy = sortBy + '_' + sortOrder;

        reviewComments = orderBy switch
        {
            "created_at_asc" => reviewComments.OrderBy(r => r.CreatedAt),
            "popularity_desc" => reviewComments.OrderByDescending(r => r.CommentReplies.Count()),
            "popularity_asc" => reviewComments.OrderBy(r => r.CommentReplies.Count()),
            _ => reviewComments.OrderByDescending(r => r.CreatedAt)
        };
        
        return await reviewComments
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    
    public async Task<ICollection<Comment>> GetCommentReplies(
        int commentId,
        int page,
        int pageSize,
        string sortBy,
        string sortOrder)
    {
        var commentReplies =
            from comment in _dbSet
            where comment.ParentCommentId == commentId
            select comment;
        
        var orderBy = sortBy + '_' + sortOrder;

        commentReplies = orderBy switch
        {
            "created_at_asc" => commentReplies.OrderBy(r => r.CreatedAt),
            "popularity_desc" => commentReplies.OrderByDescending(r => r.CommentReplies.Count()),
            "popularity_asc" => commentReplies.OrderBy(r => r.CommentReplies.Count()),
            _ => commentReplies.OrderByDescending(r => r.CreatedAt)
        };
        
        return await commentReplies
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}