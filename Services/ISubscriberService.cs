namespace let_em_cook.Services;
using let_em_cook.DTO;
using let_em_cook.Models;

public interface ISubscriberService
{
    Task<bool> SubscribeAsync(string userId, string chefId);
    Task<bool> UnsubscribeAsync(string userId, string chefId);
    Task<IEnumerable<SubscriberDto>> GetSubscribersForChefAsync(string chefId);
    Task<IEnumerable<SubscriptionDto>> GetSubscriptionsForUserAsync(string userId);

    Task NotifySubscribersAsync(Recipe recipe);
}
