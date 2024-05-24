using System;
using System.Collections.Generic;

namespace WebIcecream.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public int BookId { get; set; }

    public string Address { get; set; } = null!;

    public decimal Cost { get; set; }

    public string? PaymentOption { get; set; }

    public string? OrderStatus { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual UserAccount User { get; set; } = null!;
}
