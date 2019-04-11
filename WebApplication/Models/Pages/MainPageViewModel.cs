using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsPortal.Website.Models
{
    public class MainPageViewModel
    {
        public MainPageViewModel()
        {
            Articles = new List<ArticleViewModel>();
            LeadingArticles = new List<ArticleViewModel>();
        }
        public ICollection<ArticleViewModel> Articles { get; set; }
        public ICollection<ArticleViewModel> LeadingArticles { get; set; }
    }
}
