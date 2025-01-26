using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace let_em_cook.Models;

public class Vote
{
    public int VoteId { get; set; }
    
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; }
    
    [Required][ForeignKey("ApplicationUser")]
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    
    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; }
    
    [Required]
    public VoteType VoteType { get; set; }
    
}

public enum VoteType{
    Upvote,
    Downvote
}