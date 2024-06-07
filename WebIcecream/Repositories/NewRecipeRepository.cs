using WebIcecream.Models;
using Microsoft.EntityFrameworkCore;

namespace WebIcecream.Data.Repositories;

public interface INewRecipeRepository
{
    Task<NewRecipe> AddNewRecipeAsync(NewRecipe newrecipe);
    Task<NewRecipe> UpdateNewRecipeAsync(NewRecipe newrecipe);
    Task<IEnumerable<NewRecipe>> GetNewRecipesAsync();
    Task<NewRecipe?> FindNewRecipeByIdAsync(int id);
    Task DeleteNewRecipeAsync(NewRecipe newrecipe);
}

public class NewRecipeRepository : INewRecipeRepository
{
    private readonly ProjectDak3Context _context;

    public NewRecipeRepository(ProjectDak3Context context)
    {
        _context = context;
    }

    public async Task<NewRecipe> AddNewRecipeAsync(NewRecipe newrecipe)
    {
        _context.NewRecipes.Add(newrecipe);
        await _context.SaveChangesAsync();
        return newrecipe;
    }

    public async Task<NewRecipe> UpdateNewRecipeAsync(NewRecipe newrecipe)
    {
        _context.NewRecipes.Update(newrecipe);
        await _context.SaveChangesAsync();
        return newrecipe;
    }

    public async Task DeleteNewRecipeAsync(NewRecipe newrecipe)
    {
        _context.NewRecipes.Remove(newrecipe);
        await _context.SaveChangesAsync();
    }

    public async Task<NewRecipe?> FindNewRecipeByIdAsync(int id)
    {
        var newrecipe = await _context.NewRecipes.FindAsync(id);
        return newrecipe;
    }

    public async Task<IEnumerable<NewRecipe>> GetNewRecipesAsync()
    {
        return await _context.NewRecipes.ToListAsync();
    }
}
