using System.ComponentModel.DataAnnotations;

namespace let_em_cook.DTO;

public class CommentDto
{
    public int? CommentId { get; set; }
    
    [MaxLength(1000)]
    public string Content { get; set; }
    
    public int? ReviewId { get; set; }
    
    public int? ParentCommentId { get; set; }
    
    public DateTime? CreatedAt { get; set; }
}