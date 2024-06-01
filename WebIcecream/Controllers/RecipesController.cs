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
        public async Task<IActionResult> GetRecipes()
        {
            var recipes = await _recipeRepo.GetRecipesAsync();
            return Ok(recipes);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecipe([FromForm] RecipeDTO recipeToAdd)
        {
            try
            {
                if (recipeToAdd.ImageFile?.Length > 1 * 1024 * 1024)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "File size should not exceed 1 MB");
                }

                string[] allowedFileExtensions = { ".jpg", ".jpeg", ".png" };
                string createdImageName = await _fileService.SaveFileAsync(recipeToAdd.ImageFile, allowedFileExtensions);

                var recipe = new Recipe
                {
                    Flavor = recipeToAdd.Flavor,
                    Procedure = recipeToAdd.Procedure,
                    Ingredients = recipeToAdd.Ingredients,
                    ImageUrl = createdImageName
                };

                var createdRecipe = await _recipeRepo.AddRecipeAsync(recipe);
                return CreatedAtAction(nameof(CreateRecipe), createdRecipe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRecipe(int id, [FromForm] RecipeUpdateDTO recipeToUpdate)
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
                    if (recipeToUpdate.ImageFile?.Length > 1 * 1024 * 1024)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, "File size should not exceed 1 MB");
                    }

                    string[] allowedFileExtensions = { ".jpg", ".jpeg", ".png" };
                    string createdImageName = await _fileService.SaveFileAsync(recipeToUpdate.ImageFile, allowedFileExtensions);
                    recipeToUpdate.ImageUrl = createdImageName;
                }

                existingRecipe.Flavor = recipeToUpdate.Flavor;
                existingRecipe.Procedure = recipeToUpdate.Procedure;
                existingRecipe.Ingredients = recipeToUpdate.Ingredients;
                existingRecipe.ImageUrl = recipeToUpdate.ImageUrl;

                var updatedRecipe = await _recipeRepo.UpdateRecipeAsync(existingRecipe);

                if (recipeToUpdate.ImageFile != null)
                    _fileService.DeleteFile(oldImage);

                return Ok(updatedRecipe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRecipe(int id)
        {
            var recipe = await _recipeRepo.FindRecipeByIdAsync(id);
            if (recipe == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, $"Recipe with ID: {id} does not found");
            }
            return Ok(recipe);
        }

       
    }
}
