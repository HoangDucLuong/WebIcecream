namespace WebIcecream_FE_ADMIN.Models
{
    public class UserViewModel
    {
        public int UserId { get; set; }

        public string? FullName { get; set; }

        public DateTime? Dob { get; set; }

        public string? Address { get; set; }

        public string? Gender { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? PaymentStatus { get; set; }

        public DateTime? RegistrationDate { get; set; }

        public bool IsActive { get; set; }
    }
}
