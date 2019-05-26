using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace NewsPortal.Persistence.DTO
{
    public class ArticleDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Lead { get; set; }
        public string Content { get; set; }
        public IList<ImageDTO> Images { get; set; }
        public bool IsHighlighted { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? HighlightedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Author { get; set; }

        public ArticleDTO() { }
        public ArticleDTO(ArticleDTO data):this()
        {
            Id = data.Id;
            Name = data.Name;
            Lead = data.Lead;
            Content = data.Content;
            Images = new List<ImageDTO>(data.Images);
            IsHighlighted = data.IsHighlighted;
            IsPublished = data.IsPublished;
            HighlightedAt = data.HighlightedAt;
            PublishedAt = data.PublishedAt;
            Author = data.Author;
        }
        
    }
}
