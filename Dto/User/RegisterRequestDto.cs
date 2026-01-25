using System;
using System.Collections.Generic;
using System.Text;

namespace Dto.User
{
    public class RegisterRequestDto
    {
        public required string UserName { get; set; }
        public string? Email { get; set; }
        public required string Password { get; set; }
    }
}
