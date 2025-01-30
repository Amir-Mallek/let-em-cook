namespace let_em_cook.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using let_em_cook.Models;

public interface IElasticsearchService
{
    Task<bool> AddOrUpdateRecipe(Recipe recipe);
    Task<Recipe> GetRecipe(string key);
    Task<IEnumerable<Recipe>> SearchRecipe(string query);
    Task<bool> RemoveRecipe(string key);
}
