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

    [HttpPost("subscribe/{userId}/{chefId}")]
    public async Task<IActionResult> SubscribeUserToChef(string userId, string chefId)
    {
        try
        {
            var result = await _subscriberService.SubscribeAsync(userId, chefId);
            return Ok(new { Success = result, Message = result ? "Subscription successful" : "Error while subscribing." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Message = ex.Message });
        }
    }

    [HttpDelete("unsubscribe/{userId}/{chefId}")]
    public async Task<IActionResult> UnsubscribeUserFromChef(string userId, string chefId)
    {
        try
        {
            var result = await _subscriberService.UnsubscribeAsync(userId, chefId);
            return Ok(new { Success = result, Message = result ? "Unsubscription successful" : "Error while unsubscribing." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Message = ex.Message });
        }
    }

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