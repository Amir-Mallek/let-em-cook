using System.ComponentModel.DataAnnotations;

namespace let_em_cook.Models;

public class Ingredient
{
    public int IngredientId { get; set; }
    
    [Required][MaxLength(30)]
    public string Name { get; set; }
    
    [Required]
    public string ImageUrl { get; set; }
    
    public ICollection<RecipeIngredient>? Recipes { get; set; }
    
}