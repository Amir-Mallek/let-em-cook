using let_em_cook.Models.Queue;
using let_em_cook.Services.Queues;

namespace let_em_cook.BackgroundServices;

public class RecipePublicationProcessingService : BackgroundService
{
    private readonly IRecipePublicationQueueService _pubQueue;
    private readonly IEmailQueueService _emailQueue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RecipePublicationProcessingService> _logger;

    public RecipePublicationProcessingService(
        IRecipePublicationQueueService pubQueue,
        IEmailQueueService emailQueue,
        IServiceScopeFactory scopeFactory,
        ILogger<RecipePublicationProcessingService> logger)
    {
        _pubQueue = pubQueue;
        _emailQueue = emailQueue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var task = await _pubQueue.DequeuePublicationTaskAsync();
            if (task?.Recipe != null)
            {
                var chef = task.Recipe.Chef;
                if (chef?.Subscribers != null)
                {
                    foreach (var subscriber in chef.Subscribers)
                    {
                        await _emailQueue.EnqueueEmailTaskAsync(new EmailTask
                        {
                            SubscriberEmail = subscriber.Email,
                            SubscriberName = subscriber.UserName,
                            RecipeTitle = task.Recipe.Name,
                            ChefName = chef.UserName,
                            RecipeImageUrl = task.Recipe.ImageUrl,
                            RecipeDescription = task.Recipe.Description,
                            RecipeUrl = $"https://localhost:5001/recipes/{task.Recipe.RecipeId}",  // Change this URL to the actual URL of the recipe
                        });
                    }
                    _logger.LogInformation($"Recipe {task.Recipe.Name} published by {chef.UserName} has been sent to {chef.Subscribers.Count} subscribers");
                }
                else
                {
                    _logger.LogInformation(
                        $"Recipe {task.Recipe.Name} published by {chef.UserName} has no subscribers");
                }
            }
            await Task.Delay(1000, stoppingToken);  // Wait 1 second before checking the queue again
        }
    }
}