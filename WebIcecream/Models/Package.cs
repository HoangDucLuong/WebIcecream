namespace WebIcecream.Models
{
    public class Package
    {
        public int PackageID { get; set; }
        public string PackageName { get; set; } = null!;
        public double Price { get; set; }
        public int DurationDays { get; set; }


    }
}
