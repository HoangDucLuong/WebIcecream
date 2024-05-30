using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using WebIcecream.DTOs; // Make sure this using directive matches your project's structure

namespace WebIcecream.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        // This is a mock database storage for the sake of demonstration
        private static List<RecipeDTO> _recipesInMemoryDb = new List<RecipeDTO>();

        // GET: api/recipes
        [HttpGet]
        public ActionResult<IEnumerable<RecipeDTO>> GetRecipes()
        {
            // This would typically be a database call
            return Ok(_recipesInMemoryDb);
        }

        // GET: api/recipes/5
        [HttpGet("{id}")]
        public ActionResult<RecipeDTO> GetRecipe(int id)
        {
            // This would typically be a database call
            var recipe = _recipesInMemoryDb.FirstOrDefault(r => r.RecipeId == id);
            if (recipe == null)
            {
                return NotFound("Recipe not found.");
            }
            return Ok(recipe);
        }

        // POST: api/recipes
        [HttpPost]
        public ActionResult<RecipeDTO> PostRecipe(RecipeDTO recipeDto)
        {
            // In a real scenario, you would also include validation and error handling
            // This ID assignment mimics an auto-incrementing primary key in a database
            recipeDto.RecipeId = _recipesInMemoryDb.Any() ? _recipesInMemoryDb.Max(r => r.RecipeId) + 1 : 1;
            _recipesInMemoryDb.Add(recipeDto);
            // Redirect to GetRecipe route to retrieve the recipe by the newly assigned ID
            return CreatedAtAction(nameof(GetRecipe), new { id = recipeDto.RecipeId }, recipeDto);
        }

        // PUT: api/recipes/5
        [HttpPut("{id}")]
        public IActionResult PutRecipe(int id, RecipeDTO recipeDto)
        {
            var recipe = _recipesInMemoryDb.FirstOrDefault(r => r.RecipeId == id);
            if (recipe == null)
            {
                return NotFound("Recipe not found.");
            }
            recipe.Flavor = recipeDto.Flavor;
            recipe.Ingredients = recipeDto.Ingredients;
            recipe.Procedure = recipeDto.Procedure;
            recipe.ImageUrl = recipeDto.ImageUrl;

            return NoContent(); // 204 No Content is typically returned when the update is successful
        }

        // DELETE: api/recipes/5
        [HttpDelete("{id}")]
        public IActionResult DeleteRecipe(int id)
        {
            var recipe = _recipesInMemoryDb.FirstOrDefault(r => r.RecipeId == id);
            if (recipe != null)
            {
                _recipesInMemoryDb.Remove(recipe);
                return Ok();
            }
            return NotFound("Recipe not found.");
        }
    }
}
