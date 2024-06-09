using System;
using System.Collections.Generic;

namespace WebIcecream.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string ShippingAddress { get; set; } = null!;

    public int BookId { get; set; }

    public decimal Cost { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual UserAccount User { get; set; } = null!;
}
