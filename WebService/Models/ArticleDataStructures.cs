using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsPortal.WebService.Models.DataStructures
{
    public enum ArticleType
    {
        Highlighted,
        Published,
        Unpublished,
        All
    }

    [Flags]
    public enum ArticleSearchType
    {
        Name = 1,
        Lead = 2,
        Content = 4,
        All = 7
    }

    public struct ArticleApiOptions
    {
        public Int32? Offset { get; set; }
        public Int32? Limit { get; set; }
        public Boolean? IncludeImages { get; set; }
        public Boolean? IncludeAuthor { get; set; }
        public String SearchString { get; set; }
        public ArticleType? ArticleType { get; set; }
        public ArticleSearchType? SearchType { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }

        public ArticleApiOptions Merge(ArticleApiOptions options)
        {
            Offset = Offset ?? options.Offset;
            Limit = Limit ?? options.Limit;
            IncludeImages = IncludeImages ?? options.IncludeImages;
            SearchString = SearchString ?? options.SearchString;
            ArticleType = ArticleType ?? options.ArticleType;
            SearchType = SearchType ?? options.SearchType;
            DateStart = DateStart ?? options.DateStart;
            DateEnd = DateEnd ?? options.DateEnd;

            return this;
        }
    }
}
