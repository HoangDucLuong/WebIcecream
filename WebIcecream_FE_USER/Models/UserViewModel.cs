namespace WebIcecream_FE_USER.Models
{
    public class UserViewModel
    {
        public int UserId { get; set; }

        public string FullName { get; set; }

        public DateTime? Dob { get; set; }

        public string Address { get; set; }

        public string Gender { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string PaymentStatus { get; set; }

        public DateTime? RegistrationDate { get; set; }

        public bool IsActive { get; set; }
        public int? PackageId { get; set; }
        public string PackageName { get; set; }
        public DateTime? PackageStartDate { get; set; }
        public DateTime? PackageEndDate { get; set; }
    }
}
