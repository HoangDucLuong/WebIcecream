using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace WebIcecream_FE_ADMIN.Models
{
    public class RecipeViewModel
    {
        public int? RecipeId { get; set; }

        public string Flavor { get; set; }

        public string Ingredients { get; set; }

        public string Procedure { get; set; }

        public string ImageUrl { get; set; }
        [Required(ErrorMessage = "Please choose an image.")]
        public IFormFile Image{ get; set; }
    }
}
