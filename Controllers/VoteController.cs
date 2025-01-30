using System.Security.Claims;
using let_em_cook.Models;
using let_em_cook.Services.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace let_em_cook.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VoteController : Controller
{
    private readonly IVoteService _voteService;
    
    public VoteController(IVoteService voteService)
    {
        _voteService = voteService;
    }
    
    [HttpPost("upvote/{recipeId:int}")]
    [Authorize]
    public async Task<Vote> UpvoteRecipe(int recipeId)
    {
        var userId = User.Claims.First(c => c.Type == ClaimTypes.Name).Value;
        return await _voteService.UpvoteRecipe(recipeId, userId);
    }
    
    [HttpPost("downvote/{recipeId:int}")]
    [Authorize]
    public async Task<Vote> DownvoteRecipe(int recipeId)
    {
        var userId = User.Claims.First(c => c.Type == ClaimTypes.Name).Value;
        return await _voteService.DownvoteRecipe(recipeId, userId);
    }
    
    [HttpGet("/recipe/{recipeId:int}")]
    public async Task<IEnumerable<Vote>> GetVotes(int recipeId)
    {
        return await _voteService.GetVotes(recipeId);
    }
    
    [HttpGet("{voteId:int}")]
    public async Task<Vote> GetVote(int voteId)
    {
        return await _voteService.GetVote(voteId);
    }
    
    [HttpDelete("{voteId:int}")]
    [Authorize]
    public async Task RemoveVote(int voteId)
    {
        var userId = User.Claims.First(c => c.Type == ClaimTypes.Name).Value;
        await _voteService.RemoveVote(voteId, userId);
    }
}