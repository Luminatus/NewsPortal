using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewsPortal.Persistence;

namespace NewsPortal.Website.Models
{
    public class ImageViewModel
    {
        public Int32 Id { get; set; }

        public String Name { get; set; }

        public String Base64Data { get; set; }

        public ImageViewModel(ArticleImage image) {                        
            Id = image.Id;
            Name = image.Name;
            Base64Data = Convert.ToBase64String(image.Image);
        }

        public ImageViewModel() { }
    }
}
