using let_em_cook.Services.Queues;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;

namespace let_em_cook.BackgroundServices;

public class EmailProcessingService : BackgroundService
{
    private readonly IEmailQueueService _emailQueue;
    private readonly ILogger<EmailProcessingService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;


    public EmailProcessingService(
        IEmailQueueService emailQueue,
        ILogger<EmailProcessingService> logger,
        IServiceScopeFactory scopedFactory)
    {
        _emailQueue = emailQueue;
        _logger = logger;
        _scopeFactory = scopedFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var task = await _emailQueue.DequeueEmailTaskAsync();
            if (task != null)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        //_emailService = scope.ServiceProvider.GetRequiredService<IEmailService>(); //uncomment when service is ready
                        /*
                                            await _emailService.SendRecipeNotificationAsync(
                                                task.RecipeTitle,
                                                task.RecipeDescription,
                                                task.RecipeUrl,
                                                task.RecipeImageUrl,
                                                task.ChefName,
                                                task.SubscriberName,
                                                task.SubscriberEmail
                                                );*/
                        _logger.LogInformation($"Email sent to {task.SubscriberEmail}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send email");
                }
            }
            await Task.Delay(500, stoppingToken); // Adjust delay as needed
        }
    }
}