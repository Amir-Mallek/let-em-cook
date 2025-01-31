using let_em_cook.Models;
using let_em_cook.Models.Queue;

namespace let_em_cook.Services.Queues;

public interface IRecipePublicationQueueService
{
    Task EnqueuePublicationTaskAsync(Recipe recipe);
    Task<RecipePublicationTask?> DequeuePublicationTaskAsync();
}