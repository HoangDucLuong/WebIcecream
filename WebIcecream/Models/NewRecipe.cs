using System;
using System.Collections.Generic;

namespace WebIcecream.Models;

public partial class NewRecipe
{
    public int RecipeId { get; set; }

    public int UserId { get; set; }

    public string Flavor { get; set; } = null!;

    public string Ingredients { get; set; } = null!;

    public string Procedure { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public DateTime SubmissionDate { get; set; }

    public string? Status { get; set; }

    public virtual UserAccount User { get; set; } = null!;
}
