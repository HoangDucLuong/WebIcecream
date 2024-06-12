namespace WebIcecream_FE_ADMIN.Models
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
        public decimal Cost { get; set; }
        public string IsConfirmed { get; set; } = null;
    }
}
