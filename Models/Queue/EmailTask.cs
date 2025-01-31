namespace let_em_cook.Models.Queue;

public class EmailTask
{
    public string SubscriberEmail { get; set; }
    public string SubscriberName { get; set; }
    public string ChefName { get; set; }
    public string RecipeTitle { get; set; }
    public string RecipeUrl { get; set; }
    public string RecipeImageUrl { get; set; }
    public string RecipeDescription { get; set; }
}