using System;
using System.Collections.Generic;

namespace WebIcecream.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<UserAccount> UserAccounts { get; set; } = new List<UserAccount>();
}
