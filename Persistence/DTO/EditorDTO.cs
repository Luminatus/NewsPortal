using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace NewsPortal.Persistence.DTO
{
    public class EditorDTO
    {
        [Required] public readonly string Username;
        [Required] public readonly string Password;

        public EditorDTO(string username, string password = "")
        {
            Username = username;
            Password = password;
        }

        
    }
}
