﻿using System;

namespace WebIcecream.DTOs
{
    public class RegisterDTO
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string FullName { get; set; }

        public DateTime Dob { get; set; }

        public string Address { get; set; }

        public string Gender { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public int PackageId { get; set; }

        public DateTime PackageStartDate { get; set; }

        public DateTime PackageEndDate { get; set; }

    }
}
