using let_em_cook.Models.Email;

public interface IEmailTemplateService
{
    string GenerateNewRecipeNotification(NewRecipeNotification model);
}