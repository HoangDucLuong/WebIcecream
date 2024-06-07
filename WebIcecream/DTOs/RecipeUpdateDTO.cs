using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
namespace WebIcecream.DTOs
{
    public class RecipeUpdateDTO
    {
        [Required]
        public int RecipeId { get; set; }

        [Required(ErrorMessage = "Flavor is required")]
        public string Flavor { get; set; }

        [Required(ErrorMessage = "Ingredients are required")]
        public string Ingredients { get; set; }

        public string Procedure { get; set; }

        public string ImageUrl { get; set; }

        public IFormFile ImageFile { get; set; }
    }

}
