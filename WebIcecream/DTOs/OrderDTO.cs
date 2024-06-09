using System;

namespace WebIcecream.DTOs
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }

        public int BookId { get; set; }
<<<<<<< HEAD
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
=======

        public string Address { get; set; }

        public decimal Cost { get; set; }

        public string PaymentOption { get; set; }

        public string OrderStatus { get; set; }
>>>>>>> 9db8cf9597da4b99c8317aab2061f9006263561c
    }
}
