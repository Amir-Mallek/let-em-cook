namespace let_em_cook.Models.Email;

public class NewRecipeNotification
{
    public NewRecipeNotification(string recipeTitle, string recipeDescription, string recipeUrl, string recipeImageUrl, string chefName, string subscriberName)
    {
        RecipeTitle = recipeTitle;
        RecipeDescription = recipeDescription;
        RecipeUrl = recipeUrl;
        RecipeImageUrl = recipeImageUrl;
        ChefName = chefName;
        SubscriberName = subscriberName;
    }
    
    public string RecipeTitle { get; set; }
    public string RecipeDescription { get; set; }
    public string RecipeUrl { get; set; }
    public string RecipeImageUrl { get; set; }
    public string ChefName { get; set; }
    public string SubscriberName { get; set; }
    
}