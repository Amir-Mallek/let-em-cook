using let_em_cook.Models;

namespace let_em_cook.Services;

public class RecipeProjection
{
    public int RecipeId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Steps { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public TimeSpan Duration { get; set; }
    public int NumberOfUpvotes { get; set; }
    public int NumberOfDownvotes { get; set; }
    public int NumberOfViews { get; set; }
    public string ImageUrl { get; set; }
    public DateTime TimeOfPublishement { get; set; }
    public bool IsPublished { get; set; }
    public string UserId { get; set; }
    public ICollection<RecipeIngredient>? Ingredients { get; set; }
    public ICollection<Tag>? Tags { get; set; }
    public ICollection<Vote>? Votes { get; set; }
    public ICollection<Review>? Reviews { get; set; }
}