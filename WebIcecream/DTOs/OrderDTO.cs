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

        public string Address { get; set; } = null!;

        public decimal Cost { get; set; }

        public string PaymentOption { get; set; } = null!;

        public string OrderStatus { get; set; } = null!;
    }
}
