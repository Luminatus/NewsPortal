using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace NewsPortal.Persistence
{
    public class ArticleImage
    {
        [Key]
        public Int32 Id { get; set; }

        [Required]
        [MaxLength(64)]
        public String Name { get; set; }

        [Required]
        public Article Article { get; set; }

        [Required]
        public byte[] Image { get; set; }

    }
}