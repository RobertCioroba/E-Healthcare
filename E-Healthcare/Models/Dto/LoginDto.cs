﻿namespace E_Healthcare.Models.Dto
{
    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Type { get; set; }
    }
}
