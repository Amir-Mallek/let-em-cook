using let_em_cook.Models;
using let_em_cook.Data;
using let_em_cook.Services.Queues;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace let_em_cook.Services
{
    public interface IRecipeService
    {
        Task<RecipeDto> CreateRecipeAsync(RecipeCreateDto recipe, bool publishImmediately);
        Task<RecipeDto> GetRecipeByIdAsync(int id);
        Task<(IEnumerable<RecipeDto>, int)> GetAllRecipesAsync(int pageNumber, int pageSize);
        Task<bool> UpdateRecipeAsync(int id, Recipe updatedRecipe);
        Task<bool> DeleteRecipeAsync(int id);
    }

    public class RecipeService : IRecipeService
    {
        private readonly ApplicationdbContext _context;
        private readonly IRecipePublicationQueueService _publicationQueueService;
        private readonly IElasticsearchService _elasticsearchService;


        public RecipeService(
            ApplicationdbContext context,
            IRecipePublicationQueueService publicationQueueService,
            IElasticsearchService elasticsearchService)
        {
            _context = context;
            _publicationQueueService = publicationQueueService;
            _elasticsearchService = elasticsearchService;
        }

        // Create a new recipe and either publish immediately or schedule
        public async Task<RecipeDto> CreateRecipeAsync(RecipeCreateDto recipe, bool publishImmediately)
        {
            if (recipe == null) throw new ArgumentException("Recipe data is required.");

            var existingUser = await _context.Users.FindAsync(recipe.UserId);
            if (existingUser == null) throw new ArgumentException($"User with ID {recipe.UserId} not found.");

            var recipeObject = new Recipe
            {
                Name = recipe.Name,
                Description = recipe.Description,
                Steps = recipe.Steps,
                Difficulty = recipe.Difficulty,
                Duration = recipe.Duration,
                ImageUrl = recipe.ImageUrl,
                UserId = recipe.UserId,
                TimeOfPublishement = recipe.TimeOfPublishement ?? DateTime.Now,
                IsPublished = publishImmediately,
                Chef = existingUser
            };
            _context.Entry(existingUser).State = EntityState.Unchanged;
            _context.Recipes.Add(recipeObject);
            await _context.SaveChangesAsync();
            await _context.Entry(recipeObject).Reference(r => r.Chef).LoadAsync();
            if (publishImmediately)
            {
                await _publicationQueueService.EnqueuePublicationTaskAsync(recipeObject);
            }

            await _elasticsearchService.AddOrUpdateRecipe(recipeObject);

            return RecipeDto.FromRecipe(recipeObject);
        }

        // Get a recipe by ID
        public async Task<RecipeDto> GetRecipeByIdAsync(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.Chef) // Include Chef data
                .FirstOrDefaultAsync(r => r.RecipeId == id);

            if (recipe == null) throw new ArgumentException("Recipe not found.");
            return RecipeDto.FromRecipe(recipe);
        }


        // Get all recipes
        public async Task<(IEnumerable<RecipeDto>, int)> GetAllRecipesAsync(int pageNumber, int pageSize)
        {
            var totalCount = await _context.Recipes.CountAsync();
            var recipes = await _context.Recipes
                .Skip((pageNumber - 1) * pageSize)
                .Include(r => r.Chef)
                .Take(pageSize)
                .ToListAsync();

            return (recipes.Select(RecipeDto.FromRecipe), totalCount);
        }

        // Update an existing recipe
        public async Task<bool> UpdateRecipeAsync(int id, Recipe updatedRecipe)
        {
            if (id != updatedRecipe.RecipeId)
            {
                throw new ArgumentException("Recipe ID mismatch.");
            }

            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null) throw new ArgumentException("Recipe not found.");

            recipe.Name = updatedRecipe.Name;
            recipe.Description = updatedRecipe.Description;
            recipe.Steps = updatedRecipe.Steps;
            recipe.Difficulty = updatedRecipe.Difficulty;
            recipe.Duration = updatedRecipe.Duration;
            recipe.NumberOfUpvotes = updatedRecipe.NumberOfUpvotes;
            recipe.NumberOfDownvotes = updatedRecipe.NumberOfDownvotes;
            recipe.NumberOfViews = updatedRecipe.NumberOfViews;
            recipe.ImageUrl = updatedRecipe.ImageUrl;
            recipe.TimeOfPublishement = updatedRecipe.TimeOfPublishement;
            recipe.IsPublished = updatedRecipe.IsPublished;
            recipe.UserId = updatedRecipe.UserId;

            await _context.SaveChangesAsync();

            await _elasticsearchService.AddOrUpdateRecipe(recipe);

            return true;
        }

        // Delete a recipe
        public async Task<bool> DeleteRecipeAsync(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null) throw new ArgumentException("Recipe not found.");

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
