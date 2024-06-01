using WebIcecream.Models;
using Microsoft.EntityFrameworkCore;

namespace WebIcecream.Data.Repositories;

public interface IRecipeRepository
{
    Task<Recipe> AddRecipeAsync(Recipe recipe);
    Task<Recipe> UpdateRecipeAsync(Recipe recipe);
    Task<IEnumerable<Recipe>> GetRecipesAsync();
    Task<Recipe?> FindRecipeByIdAsync(int id);
    Task DeleteRecipeAsync(Recipe recipe);
}

public class RecipeRepository : IRecipeRepository
{
    private readonly ProjectDak3Context _context;

    public RecipeRepository(ProjectDak3Context context)
    {
        _context = context;
    }

    public async Task<Recipe> AddRecipeAsync(Recipe recipe)
    {
        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();
        return recipe;  // returning created recipe, it will automatically fetch `RecipeId`
    }

    public async Task<Recipe> UpdateRecipeAsync(Recipe recipe)
    {
        _context.Recipes.Update(recipe);
        await _context.SaveChangesAsync();
        return recipe;
    }

    public async Task DeleteRecipeAsync(Recipe recipe)
    {
        _context.Recipes.Remove(recipe);
        await _context.SaveChangesAsync();
    }

    public async Task<Recipe?> FindRecipeByIdAsync(int id)
    {
        var recipe = await _context.Recipes.FindAsync(id);
        return recipe;
    }

    public async Task<IEnumerable<Recipe>> GetRecipesAsync()
    {
        return await _context.Recipes.ToListAsync();
    }
}
