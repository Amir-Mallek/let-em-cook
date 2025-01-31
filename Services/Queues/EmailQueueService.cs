
using System.Text.Json;
using let_em_cook.Models.Queue;
using StackExchange.Redis;

namespace let_em_cook.Services.Queues;

public class EmailQueueService : IEmailQueueService
{
    private readonly IDatabase _redisDb;
    private const string QueueName = "email_tasks";

    public EmailQueueService(IConnectionMultiplexer redis)
    {
        _redisDb = redis.GetDatabase();
    }

    public async Task EnqueueEmailTaskAsync(EmailTask emailTask)
    {
        var serializedTask = JsonSerializer.Serialize(emailTask);
        await _redisDb.ListLeftPushAsync(QueueName, serializedTask);
    }

    public async Task<EmailTask?> DequeueEmailTaskAsync()
    {
        var serializedTask = await _redisDb.ListRightPopAsync(QueueName);
        return serializedTask.HasValue ? 
            JsonSerializer.Deserialize<EmailTask>(serializedTask!) : 
            null;
    }
}