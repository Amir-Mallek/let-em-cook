using let_em_cook.Models;
using let_em_cook.Repositories;
using let_em_cook.Services.ServiceContracts;

namespace let_em_cook.Services;

public class VoteService : IVoteService
{
    private readonly IRepository<Vote> _voteRepository;
    private readonly IRepository<Recipe> _recipeRepository;
    
    public VoteService(
        IRepository<Vote> voteRepository,
        IRepository<Recipe> recipeRepository)
    {
        _voteRepository = voteRepository;
        _recipeRepository = recipeRepository;
    }
    
    private async Task<Vote> CreateVote(int recipeId, string userId, VoteType voteType)
    {
        // optional check: if user has already voted on recipe
        // change vote
        
        var recipe = await _recipeRepository.GetByIdAsync(recipeId);
        if (recipe == null) throw new BadHttpRequestException("Recipe not found.");
        recipe.NumberOfUpvotes += voteType == VoteType.Upvote ? 1 : 0;
        recipe.NumberOfDownvotes += voteType == VoteType.Downvote ? 1 : 0;
        _recipeRepository.Update(recipe);
        
        var vote = new Vote
        {
            RecipeId = recipeId,
            UserId = userId,
            VoteType = voteType,
            CreatedAt = DateTime.Now
        };
        
        await _voteRepository.AddAsync(vote);
        await _voteRepository.SaveChangesAsync();
        
        return vote;
    }
    
    public async Task<Vote> UpvoteRecipe(int recipeId, string userId)
    {
        return await CreateVote(recipeId, userId, VoteType.Upvote);
    }

    public async Task<Vote> DownvoteRecipe(int recipeId, string userId)
    {
        return await CreateVote(recipeId, userId, VoteType.Downvote);
    }
    
    public async Task<Vote> GetVote(int voteId)
    {
        var vote = await _voteRepository.GetByIdAsync(voteId);
        if (vote == null) throw new BadHttpRequestException("Vote not found.");
        return vote;
    }
    public async Task<IEnumerable<Vote>> GetVotes(int recipeId)
    {
        return await _voteRepository.GetAllAsync();
    }
    
    public async Task RemoveVote(int voteId, string userId)
    {
        var vote = await _voteRepository.GetByIdAsync(voteId);
        if (vote != null && vote.UserId != userId)
        {
            throw new UnauthorizedAccessException("You are not authorized to remove this vote.");
        }

        if (vote != null)
        {
            _voteRepository.Delete(vote);
        }
    }
}