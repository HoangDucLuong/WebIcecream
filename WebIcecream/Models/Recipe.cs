using System;
using System.Collections.Generic;

namespace WebIcecream.Models;

public partial class Recipe
{
    public int RecipeId { get; set; }

    public string Flavor { get; set; } = null!;

    public string Ingredients { get; set; } = null!;

    public string Procedure { get; set; } = null!;

    public string? ImageUrl { get; set; }
}
