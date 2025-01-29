using let_em_cook.Models;

namespace let_em_cook.Repositories;


public interface IUserRepository : IRepository<ApplicationUser>
{
    Task<IEnumerable<ApplicationUser>> GetSubscribersForChefAsync(string chefId);
    Task<IEnumerable<ApplicationUser>> GetSubscriptionsForUserAsync(string userId);
}