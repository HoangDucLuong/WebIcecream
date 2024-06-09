using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace WebIcecream_FE_ADMIN.Models
{
    public class ProductViewModel
    {
        public int? BookId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public decimal Price { get; set; }
        [Required(ErrorMessage = "Please choose an image.")]
        public IFormFile Image { get; set; }
        public bool KeepCurrentImage { get; set; }
    }
}
