using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace let_em_cook.Models;

public class Comment
{
    public int CommentId { get; set; }
    
    [Required][MaxLength(1000)]
    public string Content { get; set; }
    
    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; }
    
    [Required][ForeignKey("ApplicationUser")]
    public int UserId { get; set; }
    public ApplicationUser User { get; set; }
    
    public int? ReviewId { get; set; }
    public Review Review { get; set; }
    
    [ForeignKey("Comment")]
    public int? ParentCommentId { get; set; }
    public Comment ParentComment { get; set; }
    
    public ICollection<Comment>? CommentReplies { get; set; }

}