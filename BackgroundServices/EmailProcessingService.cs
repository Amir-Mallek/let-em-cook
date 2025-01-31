using let_em_cook.Models.Email;
using let_em_cook.Services;
using let_em_cook.Services.Queues;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;

namespace let_em_cook.BackgroundServices;

public class EmailProcessingService : BackgroundService
{
    private readonly IEmailQueueService _emailQueue;
    private readonly ILogger<EmailProcessingService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    IEmailTemplateService _emailTemplateService;
    EmailService _emailService;


    public EmailProcessingService(
        IEmailQueueService emailQueue,
        ILogger<EmailProcessingService> logger,
        IServiceScopeFactory? scopedFactory,
        IEmailTemplateService emailTemplateService,
        EmailService emailService)
    {
        _emailQueue = emailQueue;
        _logger = logger;
        _scopeFactory = scopedFactory;
        _emailTemplateService = emailTemplateService;
        _emailService = emailService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var task = await _emailQueue.DequeueEmailTaskAsync(); 
            while(task != null)
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
                        var model = new NewRecipeNotification(
                            task.RecipeTitle,
                            task.RecipeDescription,
                            task.RecipeUrl,
                            task.RecipeImageUrl,
                            task.ChefName,
                            task.SubscriberName
                        );
                        await _emailService.SendEmailAsync(task.SubscriberEmail, "New Recipe Notification!!!", _emailTemplateService.GenerateNewRecipeNotification(model));
                        _logger.LogInformation($"Email sent to {task.SubscriberEmail}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send email");
                }
                task = await _emailQueue.DequeueEmailTaskAsync();
            }
            
            await Task.Delay(1000, stoppingToken); // Adjust delay as needed
        }
    }
}