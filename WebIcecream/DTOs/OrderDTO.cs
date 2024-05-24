using System;

namespace WebIcecream.DTOs
{
    public class OrderDTO
    {
        public int OrderId { get; set; }

        public int UserId { get; set; }

        public int BookId { get; set; }

        public string Address { get; set; }

        public decimal Cost { get; set; }

        public string PaymentOption { get; set; }

        public string OrderStatus { get; set; }
    }
}
