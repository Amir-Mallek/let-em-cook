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
        Task<Recipe> CreateRecipeAsync(RecipeCreateDto recipe, bool publishImmediately);
        Task<Recipe> GetRecipeByIdAsync(int id);
        Task<IQueryable<Recipe>> GetAllRecipesAsync();
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
        public async Task<Recipe> CreateRecipeAsync(RecipeCreateDto recipe, bool publishImmediately)
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

            _context.Recipes.Add(recipeObject);
            await _context.SaveChangesAsync();

            if (publishImmediately)
            {
                await _publicationQueueService.EnqueuePublicationTaskAsync(recipeObject);
            }
            
            await _elasticsearchService.AddOrUpdateRecipe(recipeObject);

            return recipeObject;
        }

        // Get a recipe by ID
        public async Task<Recipe> GetRecipeByIdAsync(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null) throw new ArgumentException("Recipe not found.");
            return recipe;
        }

        // Get all recipes
        public async Task<IQueryable<Recipe>> GetAllRecipesAsync()
        {
            return _context.Recipes.AsQueryable();
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
