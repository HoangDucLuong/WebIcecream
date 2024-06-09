using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebIcecream.Data.Repositories;
using WebIcecream.DTOs;
using WebIcecream.Models;
using WebIcecream.Service;

namespace WebIcecream.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IRecipeRepository _recipeRepo;
        private readonly ILogger<RecipesController> _logger;
        private readonly ProjectDak3Context _context;

        public RecipesController(IFileService fileService, IRecipeRepository recipeRepo, ILogger<RecipesController> logger, ProjectDak3Context context)
        {
            _fileService = fileService;
            _recipeRepo = recipeRepo;
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecipeDTO>>> GetRecipes()
        {
            var recipes = await _context.Recipes.Select(b => new RecipeDTO
            {
                RecipeId = b.RecipeId,
                Flavor = b.Flavor,
                Ingredients = b.Ingredients,
                Procedure = b.Procedure,
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
                Ingredients = b.Ingredients,
                Procedure = b.Procedure,
                ImageUrl = b.ImageUrl
            }).FirstOrDefaultAsync(b => b.RecipeId == id);

            if (recipe == null)
            {
                return NotFound();
            }

            return Ok(recipe);
        }

        [HttpPost]
        public async Task<ActionResult<RecipeDTO>> PostRecipe([FromForm] RecipeDTO recipeDTO, [FromForm] IFormFile image)
        {
            if (recipeDTO == null)
            {
                return BadRequest("Invalid recipe data");
            }

            var recipe = new Recipe
            {
                Flavor = recipeDTO.Flavor,
                Ingredients = recipeDTO.Ingredients,
                Procedure = recipeDTO.Procedure
            };

            if (image != null && image.Length > 0)
            {
                recipe.ImageUrl = await SaveImagesAsync(image);
            }

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            recipeDTO.RecipeId = recipe.RecipeId;
            recipeDTO.ImageUrl = recipe.ImageUrl;

            return CreatedAtAction(nameof(GetRecipe), new { id = recipeDTO.RecipeId }, recipeDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecipe(int id, [FromForm] RecipeUpdateDTO recipeToUpdate)
        {
            try
            {
                if (id != recipeToUpdate.RecipeId)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, $"ID in URL and form body does not match.");
                }

                var existingRecipe = await _recipeRepo.FindRecipeByIdAsync(id);
                if (existingRecipe == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, $"Recipe with ID: {id} does not found");
                }

                string oldImage = existingRecipe.ImageUrl;
                if (recipeToUpdate.ImageFile != null)
                {
                    if (recipeToUpdate.ImageFile?.Length > 5 * 1024 * 1024)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, "File size should not exceed 5 MB");
                    }

                    string[] allowedFileExtensions = { ".jpg", ".jpeg", ".png" };
                    string createdImageName = await _fileService.SaveFileAsync(recipeToUpdate.ImageFile, allowedFileExtensions);
                    recipeToUpdate.ImageUrl = createdImageName;

                    
                    existingRecipe.ImageUrl = createdImageName;
                }
                else
                {
                    
                    recipeToUpdate.ImageUrl = existingRecipe.ImageUrl;
                }

                existingRecipe.Flavor = recipeToUpdate.Flavor;
                existingRecipe.Procedure = recipeToUpdate.Procedure;
                existingRecipe.Ingredients = recipeToUpdate.Ingredients;

                var updatedRecipe = await _recipeRepo.UpdateRecipeAsync(existingRecipe);

                if (recipeToUpdate.ImageFile != null)
                    _fileService.DeleteFile(oldImage);

                return Ok(updatedRecipe);
            }
            catch (Exception ex)
            {

                return Ok();
            }
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            try
            {
                var existingRecipe = await _recipeRepo.FindRecipeByIdAsync(id);
                if (existingRecipe == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, $"Recipe with ID: {id} does not found");
                }

                await _recipeRepo.DeleteRecipeAsync(existingRecipe);
                _fileService.DeleteFile(existingRecipe.ImageUrl);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
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
