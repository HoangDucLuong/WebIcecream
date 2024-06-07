namespace WebIcecream_FE.Models
{
    public class MembershipPackageModel
    {
        public int PackageId { get; set; }

        public string PackageName { get; set; } = null!;

        public decimal Price { get; set; }

        public int DurationDays { get; set; }

    }
}
