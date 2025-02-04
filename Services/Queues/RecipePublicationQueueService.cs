using let_em_cook.Models;
using let_em_cook.Models.Queue;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using StackExchange.Redis;
using IDatabase = StackExchange.Redis.IDatabase;

namespace let_em_cook.Services.Queues;

public class RecipePublicationQueueService : IRecipePublicationQueueService
{
    private readonly IDatabase _redisDb;
    private const string QueueName = "recipe_publication_tasks";
    private readonly JsonSerializerSettings _serializerSettings;

    public RecipePublicationQueueService(IConnectionMultiplexer redis)
    {
        _redisDb = redis.GetDatabase();
        _serializerSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            TypeNameHandling = TypeNameHandling.Auto
        };
    }
    //Use this in the recipe creation controller to add a recipe to the queue (if the user wants to publish it now)
    public async Task EnqueuePublicationTaskAsync(Recipe recipe)
    {
        var task = new RecipePublicationTask { Recipe = recipe };
        var serializedTask = JsonConvert.SerializeObject(task, _serializerSettings);
        await _redisDb.ListLeftPushAsync(QueueName, serializedTask);
    }

    public async Task<RecipePublicationTask?> DequeuePublicationTaskAsync()
    {
        var serializedTask = await _redisDb.ListRightPopAsync(QueueName);
        if (!serializedTask.HasValue) return null;

        return JsonConvert.DeserializeObject<RecipePublicationTask>(
            serializedTask!, 
            _serializerSettings
        );
    }
}