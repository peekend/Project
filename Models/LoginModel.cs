﻿using System.ComponentModel.DataAnnotations;

namespace Diplom2.Models
{
    public class LoginModel
    {
        [Required]
        public string? Login { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}