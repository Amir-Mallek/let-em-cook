using let_em_cook.Data;
using let_em_cook.DTO;
using let_em_cook.Models;
using let_em_cook.Repositories;
using let_em_cook.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


//placeholder for the mailing service
public interface IMailingService
{
    Task SendEmailAsync(string email, string subject, string emailContent);
}

public class SubscriberService : ISubscriberService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMailingService _mailingService;

    private readonly UserRepository _userRepo;

    public SubscriberService(
        UserManager<ApplicationUser> userManager,
        ApplicationdbContext context) : base()
    {
        _userManager = userManager;
        _userRepo = new UserRepository(context);
    }

    public async Task<bool> SubscribeAsync(string userId, string chefId)
    {
        // Prevent self-subscription
        if (userId == chefId)
            return false;

        var user = await _userManager.Users
            .Include(u => u.Subscriptions)
            .FirstOrDefaultAsync(u => u.Id == userId);

        var chef = await _userManager.Users
            .Include(u => u.Subscribers)
            .FirstOrDefaultAsync(u => u.Id == chefId);

        if (user == null || chef == null)
            return false;

        // Check if subscription already exists
        if (user.Subscriptions.Any(u => u.Id == chefId))
            return true;

        // Add bidirectional relationship
        user.Subscriptions.Add(chef);
        chef.Subscribers.Add(user);

        // Update both entities
        var userResult = await _userManager.UpdateAsync(user);
        var chefResult = await _userManager.UpdateAsync(chef);

        return userResult.Succeeded && chefResult.Succeeded;
    }

    public async Task<bool> UnsubscribeAsync(string userId, string chefId)
    {
        var user = await _userManager.Users
            .Include(u => u.Subscriptions)
            .FirstOrDefaultAsync(u => u.Id == userId);

        var chef = await _userManager.Users
            .Include(u => u.Subscribers)
            .FirstOrDefaultAsync(u => u.Id == chefId);

        if (user == null || chef == null)
            return false;

        // Remove bidirectional relationship
        var userSubscription = user.Subscriptions.FirstOrDefault(u => u.Id == chefId);

        var chefSubscriber = chef.Subscribers.FirstOrDefault(u => u.Id == userId);

        if (userSubscription != null) user.Subscriptions.Remove(userSubscription);
        if (chefSubscriber != null) chef.Subscribers.Remove(chefSubscriber);

        // Update both entities
        var userResult = await _userManager.UpdateAsync(user);
        var chefResult = await _userManager.UpdateAsync(chef);

        return userResult.Succeeded && chefResult.Succeeded;
    }

    public async Task<IEnumerable<SubscriberDto>> GetSubscribersForChefAsync(string chefId)
    {
        IEnumerable<ApplicationUser>? subscribers;
        try {
            subscribers = await _userRepo.GetSubscribersForChefAsync(chefId);
        } catch (Exception ex) {
            throw new Exception("An error occurred while fetching subscribers", ex);
        }
        return subscribers
            .Select(u => new SubscriberDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                Country = u.Country
            })
            .ToList();
    }

    public async Task<IEnumerable<SubscriptionDto>> GetSubscriptionsForUserAsync(string userId)
    {
        IEnumerable<ApplicationUser>? subscriptions;
        try {
         subscriptions = await _userRepo.GetSubscriptionsForUserAsync(userId);
        } catch (Exception ex) {
            throw new Exception("An error occurred while fetching subscriptions", ex);
        }

        return subscriptions
            .Select(u => new SubscriptionDto
            {
                ChefId = u.Id,
                ChefName = u.UserName,
                SubscriptionDate = DateTime.UtcNow 
            })
            .ToList();
    }

    public async Task NotifySubscribersAsync(Recipe recipe)
    {
        // Get the chef's subscribers
        var subscribers = await GetSubscribersForChefAsync(recipe.UserId);

        // Get chef details
        var chef = await _userManager.FindByIdAsync(recipe.UserId);
        
        // Prepare email content
        var subject = $"{chef.UserName} published a new recipe!";
        var emailContent = $"""
            <h2>New Recipe Alert from {chef.UserName}!</h2>
            <h3>{recipe.Name}</h3>
            <p>{recipe.Description}</p>
            <p>Difficulty: {recipe.Difficulty}</p>
            <p>Cooking Time: {recipe.Duration:hh\\:mm}</p>
            <a href="/recipes/{recipe.RecipeId}">View full recipe</a>
            """;

        var emailTasks = subscribers.Select(subscriber => 
        _mailingService.SendEmailAsync(
            subscriber.Email,
            subject,
            emailContent)
        );

        await Task.WhenAll(emailTasks);
    }
}