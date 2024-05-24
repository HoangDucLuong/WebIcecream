using System;

namespace WebIcecream.DTOs
{
    public class FeedbackDTO
    {
        public int FeedbackId { get; set; }

        public int UserId { get; set; }

        public string FeedbackText { get; set; }

        public DateTime FeedbackDate { get; set; }

    }
}
