using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Sdk;
using NewsPortal.Persistence.DTO;

namespace Xunit
{
    partial class Assert
    {
        public static void Equal(ArticleListDTO expected, ArticleListDTO actual)
        {
            Equal(expected.Limit, actual.Limit);
            Equal(expected.Count, actual.Count);
            Equal(expected.Page, actual.Page);
            Equal(expected.PageCount, actual.PageCount);
            True(expected.Articles.Count == actual.Articles.Count);
            for(int i=0; i<expected.Articles.Count; i++)
            {
                Equal(expected.Articles[i], actual.Articles[i]);
            }

            //Equal(expected.Articles, actual.Articles);
        }

        public static void Equal(ArticleListElemDTO expected, ArticleListElemDTO actual)
        {
            Equal(expected.Id, actual.Id);
            Equal(expected.Author, actual.Author);
            Equal(expected.CreatedAt, actual.CreatedAt);
            Equal(expected.PublishedAt, actual.PublishedAt);
            Equal(expected.IsPublished, actual.IsPublished);
            Equal(expected.HighlightedAt, actual.HighlightedAt);
            Equal(expected.IsHighlighted, actual.IsHighlighted);
            Equal(expected.Lead, actual.Lead);
            Equal(expected.Name, actual.Name);
            Equal(expected.ImageCount, actual.ImageCount);
        }

        public static void Equal(ArticleDTO expected, ArticleDTO actual)
        {
            Equal(expected.Id, actual.Id);
            Equal(expected.Author, actual.Author);
            Equal(expected.CreatedAt, actual.CreatedAt);
            Equal(expected.PublishedAt, actual.PublishedAt);
            Equal(expected.IsPublished, actual.IsPublished);
            Equal(expected.HighlightedAt, actual.HighlightedAt);
            Equal(expected.IsHighlighted, actual.IsHighlighted);
            Equal(expected.Lead, actual.Lead);
            Equal(expected.Name, actual.Name);

            True(expected.Images.Count == actual.Images.Count);
            for (int i = 0; i < expected.Images.Count; i++)
            {
                Equal(expected.Images[i], actual.Images[i]);
            }
        }

        public static void Equal(ImageDTO expected, ImageDTO actual)
        {
            Equal(expected.Id, actual.Id);
            Equal(expected.Base64, actual.Base64);
            Equal(expected.Name, actual.Name);
        }
    }
}
