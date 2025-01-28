using let_em_cook.Models;
using let_em_cook.Services.Queues;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using let_em_cook.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace let_em_cook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly ApplicationdbContext _context;
        private readonly IRecipePublicationQueueService _publicationQueueService;

        public RecipeController(ApplicationdbContext context, IRecipePublicationQueueService publicationQueueService)
        {
            _context = context;
            _publicationQueueService = publicationQueueService;
        }

        // Create a new recipe and either publish immediately or schedule
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Recipe>> CreateRecipe([FromBody] RecipeCreateDto recipe, bool publishImmediately = false)
        {
            if (recipe == null)
            {
                return BadRequest("Recipe data is required.");
            }

            // Check if the user already exists in the context
            var existingUser = await _context.Users.FindAsync(recipe.UserId);
            if (existingUser == null)
            {
                return NotFound($"User with ID {recipe.UserId} not found.");
            }

            var recipeObject = new Recipe
            {
                Name = recipe.Name,
                Description = recipe.Description,
                Steps = recipe.Steps,
                Difficulty = recipe.Difficulty,
                Duration = recipe.Duration,
                ImageUrl = recipe.ImageUrl,
                UserId = recipe.UserId,
                TimeOfPublishement = DateTime.Now,
                IsPublished = publishImmediately
            };

            // Link the existing tracked user to the recipe
            recipeObject.Chef = existingUser;

            _context.Recipes.Add(recipeObject);
            await _context.SaveChangesAsync();

            // If published immediately, the recipe is saved and published
            if (publishImmediately)
            {
                await _publicationQueueService.EnqueuePublicationTaskAsync(recipeObject);
            }

            return CreatedAtAction(nameof(GetRecipe), new { id = recipeObject.RecipeId }, recipeObject);
        }

        // Get a list of all recipes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipes()
        {
            var recipes = await _context.Recipes.ToListAsync();
            return Ok(recipes);
        }

        // Get a specific recipe by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Recipe>> GetRecipe(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound("Recipe not found.");
            }
            return Ok(recipe);
        }

        // Update an existing recipe
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRecipe(int id, [FromBody] Recipe updatedRecipe)
        {
            if (id != updatedRecipe.RecipeId)
            {
                return BadRequest("Recipe ID mismatch.");
            }

            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound("Recipe not found.");
            }

            // Update the recipe properties
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

            // Save changes to the database
            await _context.SaveChangesAsync();

            return NoContent(); // 204 No Content indicates successful update with no response body
        }

        // Delete a recipe
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound("Recipe not found.");
            }

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 No Content indicates successful deletion
        }
    }
}
