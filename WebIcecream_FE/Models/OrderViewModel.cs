using System;

namespace WebIcecream_FE.ViewModels
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string ShippingAddress { get; set; } = null!;
        public int BookId { get; set; }
        public string Address { get; set; } = null!;
        public decimal Cost { get; set; }
        public BookViewModel Book { get; set; } = null!;
        public UserViewModel User { get; set; } = null!;
    }

    public class BookViewModel
    {
        public int BookId { get; set; }
        public string Title { get; set; } = null!;
        // Add other relevant properties
    }

    public class UserViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        // Add other relevant properties
    }
}
