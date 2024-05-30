namespace WebIcecream_FE_ADMIN.Models
{
    public class ProductViewModel
    {
        public int? BookId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public decimal Price { get; set; }
        public IFormFile Image { get; set; }        
    }
}
