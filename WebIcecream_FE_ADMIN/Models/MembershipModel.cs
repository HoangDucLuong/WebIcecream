namespace WebIcecream_FE_ADMIN.Models
{
    public class MembershipModel
    {
        public int? PackageId { get; set; }

        public string PackageName { get; set; } = null!;

        public decimal Price { get; set; }

        public int DurationDays { get; set; }

    }
}
