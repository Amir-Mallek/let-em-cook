using Microsoft.AspNetCore.Mvc;
using let_em_cook.Services;

[ApiController]
[Route("api/[controller]")]
public class SubscribersController : ControllerBase
{
    private readonly ISubscriberService _subscriberService;

    public SubscribersController(ISubscriberService subscriberService)
    {
        _subscriberService = subscriberService;
    }

    // Subscribe a user to a chef
    [HttpPost("subscribe/{userId}/{chefId}")]
    public async Task<IActionResult> SubscribeUserToChef(string userId, string chefId)
    {
        try
        {
            var result = await _subscriberService.SubscribeAsync(userId, chefId);
            return Ok(new { Success = result, Message = "Subscription successful" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Message = ex.Message });
        }
    }

    // Unsubscribe a user from a chef
    [HttpDelete("unsubscribe/{userId}/{chefId}")]
    public async Task<IActionResult> UnsubscribeUserFromChef(string userId, string chefId)
    {
        try
        {
            var result = await _subscriberService.UnsubscribeAsync(userId, chefId);
            return Ok(new { Success = result, Message = "Unsubscription successful" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Message = ex.Message });
        }
    }

    // Get all subscribers for a chef
    [HttpGet("chef/{chefId}/subscribers")]
    public async Task<IActionResult> GetChefSubscribers(string chefId)
    {
        try
        {
            var subscribers = await _subscriberService.GetSubscribersForChefAsync(chefId);
            return Ok(subscribers);
        }
        catch (Exception ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    // Get all subscriptions for a user
    [HttpGet("user/{userId}/subscriptions")]
    public async Task<IActionResult> GetUserSubscriptions(string userId)
    {
        try
        {
            var subscriptions = await _subscriberService.GetSubscriptionsForUserAsync(userId);
            return Ok(subscriptions);
        }
        catch (Exception ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }
}