namespace let_em_cook.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using let_em_cook.Models;

public interface IElasticsearchService
{
    Task IndexRecipeAsync(Recipe recipe);
    Task<IEnumerable<Recipe>> SearchRecipesAsync(string query);
}
