using System;
using System.Collections.Generic;

namespace WebIcecream.Models;

public partial class Book
{
    public int BookId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public decimal Price { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
