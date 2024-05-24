using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WebIcecream.DTOs;

namespace WebIcecream.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewRecipesController : ControllerBase
    {
        private static List<NewRecipeDTO> _recipes = new List<NewRecipeDTO>(); // In-memory store for new recipes

        // GET: api/newrecipes
        [HttpGet]
        public ActionResult<IEnumerable<NewRecipeDTO>> GetAllRecipes()
        {
            return Ok(_recipes);
        }

        // GET: api/newrecipes/{id}
        [HttpGet("{id}")]
        public ActionResult<NewRecipeDTO> GetRecipeById(int id)
        {
            var recipe = _recipes.FirstOrDefault(r => r.RecipeId == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return recipe;
        }

        // POST: api/newrecipes
        [HttpPost]
        public ActionResult<NewRecipeDTO> AddNewRecipe([FromBody] NewRecipeDTO newRecipeDto)
        {
            newRecipeDto.RecipeId = _recipes.Any() ? _recipes.Max(r => r.RecipeId) + 1 : 1;
            newRecipeDto.SubmissionDate = DateTime.UtcNow; // Set the submission date to the current UTC time
            newRecipeDto.Status = "Pending"; // Initial status can be set as 'Pending' or as per business logic
            _recipes.Add(newRecipeDto);
            return CreatedAtAction(nameof(GetRecipeById), new { id = newRecipeDto.RecipeId }, newRecipeDto);
        }

        // PUT: api/newrecipes/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateRecipe(int id, [FromBody] NewRecipeDTO recipeDto)
        {
            var index = _recipes.FindIndex(r => r.RecipeId == id);
            if (index < 0)
            {
                return NotFound();
            }

            recipeDto.RecipeId = id; // Ensure the ID is correct
            _recipes[index] = recipeDto;
            return NoContent();
        }

        // DELETE: api/newrecipes/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteRecipe(int id)
        {
            var index = _recipes.FindIndex(r => r.RecipeId == id);
            if (index < 0)
            {
                return NotFound();
            }

            _recipes.RemoveAt(index);
            return NoContent();
        }
    }

}
