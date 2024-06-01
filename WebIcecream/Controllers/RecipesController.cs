using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebIcecream.DTOs;

namespace WebIcecream.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly ProjectDak3Context _context;

        public RecipesController(ProjectDak3Context context)
        {
            _context = context;
        }

        // GET: api/Recipes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecipeDTO>>> GetRecipes()
        {
            var recipes = await _context.Recipes.Select(b => new RecipeDTO
            {
               RecipeId = b.RecipeId,
               Flavor = b.Flavor,
               Procedure = b.Procedure,
               Ingredients = b.Ingredients,
               ImageUrl = b.ImageUrl
            }).ToListAsync();

            return Ok(recipes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeDTO>> GetRecipe(int id)
        {
            var recipe = await _context.Recipes.Select(b => new RecipeDTO
            {
                RecipeId = b.RecipeId,
                Flavor = b.Flavor,
                Procedure = b.Procedure,
                Ingredients = b.Ingredients,
                ImageUrl = b.ImageUrl
            }).FirstOrDefaultAsync(b => b.RecipeId == id);

            if (recipe == null)
            {
                return NotFound();
            }

            return Ok(recipe);
        }

        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.RecipeId == id);
        }

        private async Task<string> SaveImageAsync(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return null;
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine("wwwroot/images", fileName);

            // Create directory if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            var imageUrl = $"{baseUrl}/images/{fileName}";

            return imageUrl;
        }

        [HttpPost]
        public async Task<ActionResult<RecipeDTO>> PostRecipes([FromForm] RecipeDTO recipeDTO, [FromForm] IFormFile image)
        {
            if (recipeDTO == null)
            {
                return BadRequest("Invalid recipe data");
            }

            var recipe = new Recipe
            {
                Flavor = recipeDTO.Flavor,
                Procedure = recipeDTO.Procedure,
                Ingredients = recipeDTO.Ingredients,
            };

            if (image != null && image.Length > 0)
            {
                recipe.ImageUrl = await SaveImagesAsync(image);
            }

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            recipeDTO.RecipeId = recipe.RecipeId;
            recipeDTO.ImageUrl = recipe.ImageUrl;

            return CreatedAtAction(nameof(GetRecipe), new { id = recipe.RecipeId }, recipeDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecipes(int id, [FromForm] RecipeDTO recipeDTO, [FromForm] IFormFile image)
        {
            if (id != recipeDTO.RecipeId)
            {
                return BadRequest();
            }

            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }

            recipe.Flavor = recipeDTO.Flavor;
            recipe.Procedure = recipeDTO.Procedure;
            recipe.Ingredients = recipeDTO.Ingredients;

            if (image != null)
            {
                recipe.ImageUrl = await SaveImagesAsync(image);
            }

            _context.Entry(recipe).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecipeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipes(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<string> SaveImagesAsync(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return null;
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine("wwwroot/images", fileName);

            // Create directory if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            var imageUrl = $"{baseUrl}/images/{fileName}";

            return imageUrl;
        }

    }
}
