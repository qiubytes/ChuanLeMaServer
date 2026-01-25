using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class AppUser
    {
        public required string Id { get; set; }
        public required string UserName { get; set; }
        public string? Email { get; set; }
        public required string Password { get; set; }
    }
}
