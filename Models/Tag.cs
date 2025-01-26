using System.ComponentModel.DataAnnotations;

namespace let_em_cook.Models;

public class Tag
{
    public int TagId { get; set; }
    
    [Required][MaxLength(20)]
    public string Name { get; set; }
    
    public ICollection<Recipe>? Recipes { get; set; }
}