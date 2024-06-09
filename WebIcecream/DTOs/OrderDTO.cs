using System;

namespace WebIcecream.DTOs
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string ShippingAddress { get; set; } = null!;
        public int BookId { get; set; }
        public decimal Cost { get; set; }
        public decimal Price { get; set; }

        public OrderDTO(BookDTO book)
        {
            if (book != null)
            {
                BookId = book.BookId;         
                Price = book.Price;
                Cost = book.Price; 
            }
        }
        
        public OrderDTO()
        {
        }
    }
}
