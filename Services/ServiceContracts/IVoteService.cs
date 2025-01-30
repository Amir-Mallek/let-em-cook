using let_em_cook.Models;

namespace let_em_cook.Services.ServiceContracts;

public interface IVoteService
{
    public Task<Vote> UpvoteRecipe(int recipeId, string userId);

    public Task<Vote> DownvoteRecipe(int recipeId, string userId);
    
    public Task<IEnumerable<Vote>> GetVotes(int recipeId);
    
    public Task<Vote> GetVote(int voteId);

    public Task RemoveVote(int voteId, string userId);
}