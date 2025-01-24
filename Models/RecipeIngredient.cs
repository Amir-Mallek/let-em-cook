using System.ComponentModel.DataAnnotations;

namespace let_em_cook.Models;

public class RecipeIngredient
{
    public int RecipeIngredientId { get; set; }
    
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; }
    
    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; }
    
    [Required]
    public string Quantity { get; set; }
    
    public string? Unit { get; set; }
    
}