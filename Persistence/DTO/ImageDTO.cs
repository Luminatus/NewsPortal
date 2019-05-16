using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;


namespace NewsPortal.Persistence.DTO
{
    public class ImageDTO
    {
        [Required] public int Id { get; set; }
        [Required] public string Base64 { get; set; }
    }
}
