using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewsPortal.Persistence;

namespace NewsPortal.Website.Models
{
    public class ArticleViewModel
    {
        public Int32 Id { get; set; }

        public String Name { get; set; }

        public String Lead { get; set; }

        public String Content { get; set; }

        public Boolean IsHighlighted { get; set; }

        public String AuthorName { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? PublishedAt { get; set; }

        public DateTime? HighlightedAt { get; set; }

        public ICollection<ImageViewModel> Images { get; set; }

        public ImageViewModel CoverImage
        {
            get
            {
                return Images.FirstOrDefault();
            }
        }

        public ArticleViewModel(Article article):this()
        {
            Id = article.Id;
            Name = article.Name;
            Lead = article.Lead;
            Content = article.Content;
            IsHighlighted = article.IsHighlighted;
            AuthorName = article.Author != null ? article.Author.Name : "anon";
            CreatedAt = article.CreatedAt;
            UpdatedAt = article.UpdatedAt;
            PublishedAt = article.PublishedAt;
            HighlightedAt = article.HighlightedAt;
            foreach(var image in article.Images)
            {
                Images.Add(new ImageViewModel(image));
            }
        }

        public ArticleViewModel()
        {
            Images = new List<ImageViewModel>();
        }
    }
}
