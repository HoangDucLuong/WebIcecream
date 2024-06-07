using System;
using System.Collections.Generic;

namespace WebIcecream.Models;

public partial class MembershipPackage
{
    public int PackageId { get; set; }

    public string PackageName { get; set; } = null!;

    public decimal Price { get; set; }

    public int DurationDays { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
