using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsPortal.Persistence.DTO;
using System.Collections.ObjectModel;

namespace NewsPortal.DesktopApplication.Model
{
    public class ArticleClientData: ArticleDTO
    {
        public new IList<ImageClientData> Images { get; set; }
        public Boolean IsUploaded { get; set; } = false;

        public ArticleClientData() : base() { }
        public ArticleClientData(ArticleDTO data):base()
        {
            Id = data.Id;
            Name = data.Name;
            Lead = data.Lead;
            Content = data.Content;
            IsHighlighted = data.IsHighlighted;
            IsPublished = data.IsPublished;
            HighlightedAt = data.HighlightedAt;
            PublishedAt = data.PublishedAt;
            CreatedAt = data.CreatedAt;
            Author = data.Author;

            Images = new ObservableCollection<ImageClientData>();
            foreach (var image in data.Images)
            {
                Images.Add(new ImageClientData(image));
            }
        }

        public ArticleClientData(ArticleClientData data) : this((ArticleDTO)data)
        {
            IsUploaded = data.IsUploaded;
        }
    }
}
