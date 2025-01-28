using let_em_cook.Models.Queue;

namespace let_em_cook.Services.Queues;

public interface IEmailQueueService
{
    Task EnqueueEmailTaskAsync(EmailTask emailTask);
    Task<EmailTask?> DequeueEmailTaskAsync();
}