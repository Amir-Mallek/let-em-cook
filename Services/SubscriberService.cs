using let_em_cook.Data;
using let_em_cook.DTO;
using let_em_cook.Models;
using let_em_cook.Repositories;
using let_em_cook.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;



public class SubscriberService : ISubscriberService
{
    private readonly UserManager<ApplicationUser> _userManager;

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

        if (user.Subscriptions.Any(u => u.Id == chefId))
            return true;

        user.Subscriptions.Add(chef);
        chef.Subscribers.Add(user);

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

        var userSubscription = user.Subscriptions.FirstOrDefault(u => u.Id == chefId);

        var chefSubscriber = chef.Subscribers.FirstOrDefault(u => u.Id == userId);

        if (userSubscription != null) user.Subscriptions.Remove(userSubscription);
        if (chefSubscriber != null) chef.Subscribers.Remove(chefSubscriber);

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
}