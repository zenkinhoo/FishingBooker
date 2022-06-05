using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models.DTO
{
    public class LoginDTO
    {
        public string email { get; set; }
        public string password { get; set; }

        public LoginDTO(string Email, string Password)
        {
            email = Email;
            password = Password;
        }
    }
}
