using let_em_cook.Models.Email;

public class EmailTemplateService : IEmailTemplateService
{
    public string GenerateNewRecipeNotification(NewRecipeNotification model)
    {
        return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        .container {{ max-width: 600px; margin: 20px auto; font-family: Arial, sans-serif; }}
                        .header {{ color: #2c3e50; border-bottom: 2px solid #ecf0f1; padding-bottom: 10px; }}
                        .recipe-image {{ max-width: 100%; height: auto; border-radius: 8px; margin: 15px 0; }}
                        .button {{ 
                            background-color: #3498db; 
                            color: white; 
                            padding: 12px 24px; 
                            text-decoration: none; 
                            border-radius: 5px; 
                            display: inline-block;
                        }}
                        .footer {{ margin-top: 20px; color: #7f8c8d; font-size: 0.9em; }}
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <h1 class=""header"">👨🍳 New Recipe from {model.ChefName}!</h1>
                        
                        <h2>{model.RecipeTitle}</h2>
                        
                        {(string.IsNullOrEmpty(model.RecipeImageUrl) ? "" : $@"<img src=""{model.RecipeImageUrl}"" alt=""{model.RecipeTitle}"" class=""recipe-image"">")}

                        <p>{model.RecipeDescription}</p>

                        <a href=""{model.RecipeUrl}"" class=""button"">View Full Recipe</a>

                        <div class=""footer"">
                            <p>Hi {model.SubscriberName},</p>
                            <p>You're receiving this email because you're following {model.ChefName} on RecipePlatform.</p>
                            <p>Want to change your notification preferences? <a href=""#"">Manage subscriptions</a></p>
                            <p>© {DateTime.Now.Year} RecipePlatform.</p>
                        </div>
                    </div>
                </body>
                </html>";
    }
}