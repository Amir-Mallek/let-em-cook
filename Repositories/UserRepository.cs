using let_em_cook.Data;
using let_em_cook.Models;
using Microsoft.EntityFrameworkCore;

namespace let_em_cook.Repositories;


public class UserRepository : Repository<ApplicationUser>, IUserRepository
{
    public UserRepository(ApplicationdbContext _context) : base(_context) { }

    public async Task<IEnumerable<ApplicationUser>> GetSubscribersForChefAsync(string chefId)
    {
        return await _dbSet
            .Where(chef => chef.Id == chefId)
            .SelectMany(chef => chef.Subscribers)
            .ToListAsync();
    }

    public async Task<IEnumerable<ApplicationUser>> GetSubscriptionsForUserAsync(string userId)
    {
        return await _dbSet
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Subscriptions)
            .ToListAsync();
    }
}