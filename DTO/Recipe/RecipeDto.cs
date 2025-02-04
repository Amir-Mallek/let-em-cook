using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace let_em_cook.Models;
public class RecipeDto
{
    public int RecipeId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Steps { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public TimeSpan Duration { get; set; }
    public string ImageUrl { get; set; }
    public DateTime TimeOfPublishement { get; set; }
    public int NumberOfUpvotes { get; set; }
    public int NumberOfDownvotes { get; set; }
    public int NumberOfViews { get; set; }
    public ICollection<Tag>? Tags { get; set; }
    public string ChefName { get; set; }
    ICollection<RecipeIngredient>? Ingredients { get; set; }

    public string ChefId { get; set; }

    public static RecipeDto FromRecipe(Recipe recipe)
    {
        return new RecipeDto
        {
            RecipeId = recipe.RecipeId,
            Name = recipe.Name,
            Description = recipe.Description,
            Steps = recipe.Steps,
            Difficulty = recipe.Difficulty,
            Duration = recipe.Duration,
            ImageUrl = recipe.ImageUrl,
            TimeOfPublishement = recipe.TimeOfPublishement,
            ChefName = recipe.Chef?.UserName,
            Tags = recipe.Tags,
            NumberOfViews = recipe.NumberOfViews,
            NumberOfUpvotes = recipe.NumberOfUpvotes,
            NumberOfDownvotes = recipe.NumberOfDownvotes,
            ChefId = recipe.Chef?.Id,
            Ingredients = recipe.Ingredients
        };
    }
}
