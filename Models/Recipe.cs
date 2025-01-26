using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace let_em_cook.Models;

public class Recipe
{
    public int RecipeId { get; set; }

    [Required]
    public string Name { get; set; }

    [Required][DataType(DataType.MultilineText)]
    public string Description { get; set; }

    [Required][DataType(DataType.MultilineText)]
    public string Steps { get; set; }

    [Required]
    public DifficultyLevel Difficulty { get; set; } // Associate the enum with a property
    
    [Required][DataType(DataType.DateTime)]
    public TimeSpan Duration { get; set; }
    
    [Required]
    public int NumberOfUpvotes { get; set; }
    
    [Required]
    public int NumberOfDownvotes { get; set; }
    
    [Required]
    public int NumberOfViews { get; set; }
    
    [Required]
    public string ImageUrl { get; set; }
    
    [Required][DataType(DataType.Time)]
    public DateTime TimeOfPublishement { get; set; }
    
    [Required][ForeignKey("ApplicationUser")]
    public string UserId { get; set; }
    public ApplicationUser Chef { get; set; }
    
    public ICollection<RecipeIngredient>? Ingredients { get; set; }
    public ICollection<Tag>? Tags { get; set; }
    public ICollection<Vote>? Votes { get; set; }
    public ICollection<Review>? Reviews { get; set; }
    
}

public enum DifficultyLevel 
{
    Easy,
    Medium,
    Hard
}