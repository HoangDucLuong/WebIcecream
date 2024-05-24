using System;
using System.Collections.Generic;

namespace WebIcecream.Models;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public int UserId { get; set; }

    public string FeedbackText { get; set; } = null!;

    public DateOnly FeedbackDate { get; set; }

    public virtual UserAccount User { get; set; } = null!;
}
