using let_em_cook.Data;
using let_em_cook.Services.Queues;
using Microsoft.EntityFrameworkCore;

namespace let_em_cook.BackgroundServices;

public class ScheduledRecipePublisher : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IRecipePublicationQueueService _pubQueue;
    private readonly ILogger<ScheduledRecipePublisher> _logger;

    public ScheduledRecipePublisher(
        IServiceScopeFactory scopeFactory,
        IRecipePublicationQueueService pubQueue,
        ILogger<ScheduledRecipePublisher> logger)
    {
        _scopeFactory = scopeFactory;
        _pubQueue = pubQueue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationdbContext>();

                var dueRecipes = await dbContext.Recipes
                    .Include(r => r.Chef)
                    .ThenInclude(c => c.Subscribers)
                    .Include(r => r.Ingredients)
                    .Include(r => r.Tags)
                    .Where(r => !r.IsPublished && r.TimeOfPublishement <= DateTime.UtcNow.AddHours(1))
                    .ToListAsync();

                if (dueRecipes.Any())
                {
                    foreach (var recipe in dueRecipes)
                    {
                        recipe.IsPublished = true;
                        await _pubQueue.EnqueuePublicationTaskAsync(recipe);
                        _logger.LogInformation($"Recipe {recipe.Name} has been added to the queue for publication");
                    }
                }
                else
                {
                    _logger.LogInformation($"No due recipes in the queue for publication ata time {DateTime.UtcNow.AddHours(1)}");
                }

                await dbContext.SaveChangesAsync();
            }
            await Task.Delay(10000, stoppingToken); // execute every minute
        }
    }
}