using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
    public class NewRecipesController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly INewRecipeRepository _newrecipeRepo;
        private readonly ILogger<NewRecipesController> _logger;
        private readonly ProjectDak3Context _context;

        public NewRecipesController(IFileService fileService, INewRecipeRepository newrecipeRepo, ILogger<NewRecipesController> logger, ProjectDak3Context context)
        {
            _fileService = fileService;
            _newrecipeRepo = newrecipeRepo;
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NewRecipeDTO>>> GetNewRecipes()
        {
            var recipes = await _context.NewRecipes.Select(b => new NewRecipeDTO
            {
                RecipeId = b.RecipeId,
                UserId = b.UserId,
                Flavor = b.Flavor,
                Ingredients = b.Ingredients,
                Procedure = b.Procedure,
                ImageUrl = b.ImageUrl,
                SubmissionDate = b.SubmissionDate,
                Status = b.Status
            }).ToListAsync();

            return Ok(recipes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NewRecipeDTO>> GetNewRecipe(int id)
        {
            var recipe = await _context.NewRecipes.Select(b => new NewRecipeDTO
            {
                RecipeId = b.RecipeId,
                UserId = b.UserId,
                Flavor = b.Flavor,
                Ingredients = b.Ingredients,
                Procedure = b.Procedure,
                ImageUrl = b.ImageUrl,
                SubmissionDate = b.SubmissionDate,
                Status = b.Status
            }).FirstOrDefaultAsync(b => b.RecipeId == id);

            if (recipe == null)
            {
                return NotFound();
            }

            return Ok(recipe);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NewRecipeDTO>>> SearchNewRecipeByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Name parameter is required.");
            }

            var newrecipes = await _context.NewRecipes
                .Where(b => b.Flavor.Contains(name))
                .Select(b => new NewRecipeDTO
                {
                    RecipeId = b.RecipeId,
                    Flavor = b.Flavor,
                    Ingredients = b.Ingredients,
                    Procedure = b.Procedure,
                    ImageUrl = b.ImageUrl,
                    SubmissionDate = b.SubmissionDate,
                    Status = b.Status,
                    
                })
                .ToListAsync();

            if (newrecipes == null || newrecipes.Count == 0)
            {
                return NotFound("No books found with the provided name.");
            }

            return Ok(newrecipes);
        }
        [HttpPost]
        public async Task<ActionResult<NewRecipeDTO>> PostNewRecipe([FromForm] NewRecipeDTO newRecipeDTO, [FromForm] IFormFile image)
        {
            if (newRecipeDTO == null)
            {
                return BadRequest("Invalid recipe data");
            }

            var recipe = new NewRecipe
            {
                UserId = newRecipeDTO.UserId,
                Flavor = newRecipeDTO.Flavor,
                Ingredients = newRecipeDTO.Ingredients,
                Procedure = newRecipeDTO.Procedure,
                SubmissionDate = newRecipeDTO.SubmissionDate,
                Status = newRecipeDTO.Status
            };

            if (image != null && image.Length > 0)
            {
                recipe.ImageUrl = await SaveImagesAsync(image);
            }

            _context.NewRecipes.Add(recipe);
            await _context.SaveChangesAsync();

            newRecipeDTO.RecipeId = recipe.RecipeId;
            newRecipeDTO.ImageUrl = recipe.ImageUrl;

            return CreatedAtAction(nameof(GetNewRecipe), new { id = newRecipeDTO.RecipeId }, newRecipeDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutNewRecipe(int id, [FromForm] UpdateNewRecipeDTO newRecipeToUpdate)
        {
            try
            {
                if (id != newRecipeToUpdate.RecipeId)
                {
                    return BadRequest("ID in URL and form body does not match.");
                }

                var existingRecipe = await _newrecipeRepo.FindNewRecipeByIdAsync(id);
                if (existingRecipe == null)
                {
                    return NotFound($"Recipe with ID: {id} not found");
                }

                string oldImage = existingRecipe.ImageUrl;
                if (newRecipeToUpdate.ImageFile != null)
                {
                    if (newRecipeToUpdate.ImageFile.Length > 5 * 1024 * 1024)
                    {
                        return BadRequest("File size should not exceed 5 MB");
                    }

                    string[] allowedFileExtensions = { ".jpg", ".jpeg", ".png" };
                    string createdImageName = await _fileService.SaveFileAsync(newRecipeToUpdate.ImageFile, allowedFileExtensions);
                    newRecipeToUpdate.ImageUrl = createdImageName;

                    existingRecipe.ImageUrl = createdImageName;
                }
                else
                {
                    newRecipeToUpdate.ImageUrl = existingRecipe.ImageUrl;
                }

                existingRecipe.Flavor = newRecipeToUpdate.Flavor;
                existingRecipe.Procedure = newRecipeToUpdate.Procedure;
                existingRecipe.Ingredients = newRecipeToUpdate.Ingredients;
                existingRecipe.SubmissionDate = newRecipeToUpdate.SubmissionDate;
                existingRecipe.Status = newRecipeToUpdate.Status;

                var updatedRecipe = await _newrecipeRepo.UpdateNewRecipeAsync(existingRecipe);

                if (newRecipeToUpdate.ImageFile != null)
                    _fileService.DeleteFile(oldImage);

                return Ok(updatedRecipe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("ApproveRecipe/{id}")]
        public async Task<IActionResult> ApproveRecipe(int id)
        {
            var existingRecipe = await _newrecipeRepo.FindNewRecipeByIdAsync(id);
            if (existingRecipe == null)
            {
                return NotFound($"Recipe with ID: {id} not found");
            }

            existingRecipe.Status = "approved";

            var newRecipe = new Recipe
            {
                Flavor = existingRecipe.Flavor,
                Ingredients = existingRecipe.Ingredients,
                Procedure = existingRecipe.Procedure,
                ImageUrl = existingRecipe.ImageUrl
            };

            _context.Recipes.Add(newRecipe);
            await _context.SaveChangesAsync();

            await _newrecipeRepo.UpdateNewRecipeAsync(existingRecipe);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNewRecipe(int id)
        {
            try
            {
                var existingRecipe = await _newrecipeRepo.FindNewRecipeByIdAsync(id);
                if (existingRecipe == null)
                {
                    return NotFound($"Recipe with ID: {id} not found");
                }

                await _newrecipeRepo.DeleteNewRecipeAsync(existingRecipe);
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
