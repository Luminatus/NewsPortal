using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace NewsPortal.Persistence.DTO
{
    public class LoginDTO
    {
        [Required] public readonly string Username;
        [Required] public readonly string Password;

        public LoginDTO(string username, string password = "")
        {
            Username = username;
            Password = password;
        }

        
    }
}
