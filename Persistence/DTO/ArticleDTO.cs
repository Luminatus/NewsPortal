using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace NewsPortal.Persistence.DTO
{
    public class ArticleDTO
    {
        [Required] public int Id { get; set; }
        [Required] public string Name { get; set; }
        [Required] public string Lead { get; set; }
        [Required] public string Content { get; set; }
        [Required] public IEnumerable<ImageDTO> Images { get; set; }
        [Required] public bool IsHighlighted { get; set; }
        [Required] public bool IsPublished { get; set; }
        [Required] public DateTime? HighlightedAt { get; set; }
        [Required] public DateTime? PublishedAt { get; set; }
        [Required] public DateTime? CreatedAt { get; set; }
        [Required] public string Author { get; set; }
    }
}
