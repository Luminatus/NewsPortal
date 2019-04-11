using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace NewsPortal.Persistence
{
    public class Article
    {
        public Article()
        {
            Images = new  List<ArticleImage>();
        }

        [Key]
        public Int32 Id { get; set; }

        [Required]
        [MaxLength(64)]
        public String Name { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public String Lead { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public String Content { get; set; }

        [Required]
        [ForeignKey("Editor")]
        public Editor Author { get; set; }

        [Required]
        public DateTime? CreatedAt { get; set; }

        [Required]
        public DateTime? UpdatedAt { get; set; }

        public DateTime? PublishedAt { get; set; }

        public DateTime? HighlightedAt { get; set; }
       
        public ICollection<ArticleImage> Images { get; set; }

        public ArticleImage CoverImage {
            get
            {
                return this.Images.FirstOrDefault();
            }
        }

        [NotMapped]
        public Boolean IsPublished {
            get
            {
                return this.PublishedAt.HasValue;
            }
            set
            {
                if(value)
                {
                    this.PublishedAt = DateTime.Now;
                }
                else
                {
                    this.PublishedAt = null;
                }
            }
        }

        [NotMapped]
        public Boolean IsHighlighted
        {
            get
            {
                return this.HighlightedAt.HasValue;
            }
            set
            {
                if (value)
                {
                    this.HighlightedAt = DateTime.Now;
                }
                else
                {
                    this.HighlightedAt = null;
                }
            }
        }

    }
}