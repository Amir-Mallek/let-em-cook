using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace let_em_cook.Models;

public class Review
{
    public int ReviewId { get; set; }
    
    [Required][MaxLength(1000)]
    public string Content { get; set; }
    
    [Required][Range(0, 5)]
    public int Rating { get; set; }
    
    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; }
    
    [Required]
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; }
    
    [Required][ForeignKey("ApplicationUser")]
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    
    public ICollection<Comment>? Comments { get; set; }
    
}