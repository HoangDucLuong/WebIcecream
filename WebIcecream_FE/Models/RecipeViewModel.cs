namespace WebIcecream_FE.Models
{
    public class RecipeViewModel
    {
        public int RecipeId { get; set; }

        public string Flavor { get; set; }

        public string Ingredients { get; set; }

        public string Procedure { get; set; }

        public string ImageUrl { get; set; }

        public IFormFile Image { get; set; }
    }
}
