﻿namespace WebIcecream_FE_ADMIN.Models
{
    public class NewRecipeViewModel
    {
        public int? RecipeId { get; set; }
        public int? UserId { get; set; }

        public string Flavor { get; set; }

        public string Ingredients { get; set; }

        public string Procedure { get; set; }

        public string ImageUrl { get; set; }
        public IFormFile Image { get; set; }
    }
}
