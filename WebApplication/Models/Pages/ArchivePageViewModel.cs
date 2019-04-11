using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsPortal.Website.Models
{
    public class ArchivePageViewModel
    {
        public ArchivePageViewModel()
        {
            Articles = new List<ArticleViewModel>();
        }
        public ICollection<ArticleViewModel> Articles { get; set; }

        public SearchModel Search { get; set; }
    }
}
