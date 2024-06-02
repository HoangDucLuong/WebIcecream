using System.ComponentModel.DataAnnotations;
namespace WebIcecream.DTOs

{
    public class RecipeDTO
    {
        [Required(ErrorMessage = "Flavor is required")]
        public string Flavor { get; set; }

        [Required(ErrorMessage = "Ingredients are required")]
        public string Ingredients { get; set; }

        [Required(ErrorMessage = "Procedure is required")]
        public string Procedure { get; set; }

        [Required(ErrorMessage = "Image file is required")]
        public IFormFile ImageFile { get; set; }
    }

    public class RecipeUpdateDTO
    {
        [Required]
        public int RecipeId { get; set; }

        [Required(ErrorMessage = "Flavor is required")]
        public string Flavor { get; set; }

        [Required(ErrorMessage = "Ingredients are required")]
        public string Ingredients { get; set; }

        [Required(ErrorMessage = "Procedure is required")]
        public string Procedure { get; set; }

        [Required(ErrorMessage = "ImageUrl file is required")]
        public string ImageUrl { get; set; }

        public IFormFile ImageFile { get; set; }
    }
}
