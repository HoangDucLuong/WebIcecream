using System;
using System.Collections.Generic;

namespace WebIcecream.Models;

public partial class UserAccount
{
    public int UserId { get; set; }

    public int? RoleId { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<NewRecipe> NewRecipes { get; set; } = new List<NewRecipe>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Role? Role { get; set; }

    public virtual User User { get; set; } = null!;
}
