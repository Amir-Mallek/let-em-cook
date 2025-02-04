using System.ComponentModel.DataAnnotations;

namespace let_em_cook.DTO;

public class ReviewDto
{
    public string Content { get; set; }
    
    [Range(0, 5)]
    public int Rating { get; set; }
    
    public int RecipeId { get; set; }
}