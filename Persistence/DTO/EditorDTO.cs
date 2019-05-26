using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using NewsPortal.Persistence;

namespace NewsPortal.Persistence.DTO
{
    public class EditorDTO
    {
        [Required] public string Username;
        [Required] public string Name;
        [Required] public string Address;
        [Required] public string Phone;
        [Required] public int ArticleCount;
    }
}
