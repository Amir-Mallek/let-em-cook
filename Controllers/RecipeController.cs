using let_em_cook.Models;
using let_em_cook.Services;
using let_em_cook.Services.Queues;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace let_em_cook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeService _recipeService;
        private readonly IElasticsearchService _elasticsearchService;


        public RecipeController(IRecipeService recipeService, IElasticsearchService elasticsearchService)
        {
            _recipeService = recipeService;
            _elasticsearchService = elasticsearchService;
        }

        // Create a new recipe and either publish immediately or schedule
        [HttpPost]
        public async Task<ActionResult<Recipe>> CreateRecipe([FromBody] RecipeCreateDto recipe, bool publishImmediately = false)
        {
            try
            {
                var recipeObject = await _recipeService.CreateRecipeAsync(recipe, publishImmediately);
                return CreatedAtAction(nameof(GetRecipe), new { id = recipeObject.RecipeId }, recipeObject);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Get a list of all recipes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipes()
        {
            var recipes = await _recipeService.GetAllRecipesAsync();
            return Ok(await recipes.ToListAsync());
        }

        // Get a specific recipe by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Recipe>> GetRecipe(int id)
        {
            try
            {
                var recipe = await _recipeService.GetRecipeByIdAsync(id);
                return Ok(recipe);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // Update an existing recipe
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRecipe(int id, [FromBody] Recipe updatedRecipe)
        {
            try
            {
                await _recipeService.UpdateRecipeAsync(id, updatedRecipe);
                return NoContent(); // 204 No Content indicates successful update with no response body
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Delete a recipe
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            try
            {
                await _recipeService.DeleteRecipeAsync(id);
                return NoContent(); // 204 No Content indicates successful deletion
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
        
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Recipe>>> SearchRecipes([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query parameter is required.");
            }

            try
            {
                var recipes = await _elasticsearchService.SearchRecipe(query);
                return Ok(recipes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while searching for recipes.");
            }
        }

    }
}
