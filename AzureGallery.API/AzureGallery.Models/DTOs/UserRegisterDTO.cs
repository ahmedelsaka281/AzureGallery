using System;
using System.Collections.Generic;
using System.Text;

namespace AzureGallery.Models.DTOs
{
    public class UserRegisterDTO
    {
        public string FullName { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
