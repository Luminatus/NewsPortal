using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Persistence;

namespace NewsPortal.Website.Services
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

    public struct ArticleServiceOptions
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

        public ArticleServiceOptions Merge(ArticleServiceOptions options)
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

    public class NewsService
    {
        private readonly NewsContext _context;
        private readonly ArticleServiceOptions OptGetArticles = new ArticleServiceOptions { Offset = 0, Limit = 0, IncludeAuthor = true, IncludeImages = false, SearchString = "", ArticleType = ArticleType.Published, SearchType = ArticleSearchType.All };
        private readonly ArticleServiceOptions OptGetArticleById = new ArticleServiceOptions { IncludeAuthor = true, IncludeImages = true };

        public enum NewsUpdateResult
        {
            Success,
            ConcurrencyError,
            DbError
        }

        public NewsService(NewsContext context)
        {
            _context = context;
        }

        #region Article

        public List<Article> GetArticles(ArticleServiceOptions? opt = null )
        {
            var options = opt.HasValue ? opt.Value.Merge(OptGetArticles) : OptGetArticles;            

            IQueryable<Article> query = _context.Articles;

            query = RestrictArticleType(query, options.ArticleType.Value);
            query = RestrictDate(query, options);
            query = RestrictSearch(query, options);

            if (options.IncludeAuthor.HasValue && options.IncludeAuthor.Value)
            {
                query = query.Include(a => a.Author);
            }

            if(options.IncludeImages.HasValue && options.IncludeImages.Value)
            {
                query = query.Include("Images");
            }

            if (options.Offset.HasValue && options.Offset.Value > 0)
            {
                query = query.Skip(options.Offset.Value);
            }

            if (options.Limit.HasValue && options.Limit.Value > 0)
            {
                query = query.Take(options.Limit.Value);
            }


            return query.ToList();


        }

        public int GetArticleCount(ArticleServiceOptions? opt = null)
        {
            var options = opt.HasValue ? opt.Value.Merge(OptGetArticles) : OptGetArticles;

            IQueryable<Article> query = _context.Articles;

            query = RestrictArticleType(query, options.ArticleType.Value);
            query = RestrictDate(query, options);
            query = RestrictSearch(query, options);


            return query.Count();

        }

        public Article GetArticleById(int id, ArticleServiceOptions? opt = null)
        {
            var options = opt.HasValue ? opt.Value.Merge(OptGetArticleById) : OptGetArticleById;

            IQueryable<Article> query = _context.Articles;

            if (options.IncludeAuthor.HasValue && options.IncludeAuthor.Value)
            {
                query = query.Include("Author");
            }
            if (options.IncludeImages.HasValue && options.IncludeImages.Value)
            {
                query = query.Include("Images");
            }

            return query.FirstOrDefault(a => a.Id == id);

        }

        public Article GetArticleById(int id, int test, bool includeAuthor = true, bool includeImages = true)
        {
            IQueryable<Article> query = _context.Articles;

            if (includeAuthor)
            {
                query = query.Include("Author");
            }
            if (includeImages)
            {
                query = query.Include("Images");
            }

            return query.FirstOrDefault(a => a.Id == id);

        }

        public bool CreateArticle(Article article)
        {
            try
            {
                _context.Add(article);
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }

        public NewsUpdateResult UpdateArticle(Article article)
        {
            try
            {
                _context.Update(article);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NewsUpdateResult.ConcurrencyError;
            }
            catch (DbUpdateException)
            {
                return NewsUpdateResult.DbError;
            }

            return NewsUpdateResult.Success;
        }

        public bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.Id == id);
        }

        public bool DeleteArticle(int id)
        {
            var article = _context.Articles.Find(id);
            if (article == null)
                return false;

            try
            {
                _context.Articles.Remove(article);
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }

        private IQueryable<Article> RestrictArticleType(IQueryable<Article> query, ArticleType type)
        {
            if (type == ArticleType.Highlighted)
            {
                query = query.Where(a => a.IsHighlighted && a.IsPublished)
                    .OrderByDescending(a => a.HighlightedAt);
            }
            else if (type == ArticleType.Published)
            {
                query = query.Where(a => a.IsPublished)
                    .OrderByDescending(a => a.PublishedAt);
            }
            else
            {
                if (type == ArticleType.Unpublished)
                {
                    query = query.Where(a => !a.IsPublished);
                }
                query = query.OrderByDescending(a => a.CreatedAt);
            }

            return query;
        }

        private IQueryable<Article> RestrictDate(IQueryable<Article> query, ArticleServiceOptions options)
        {
            bool hasStartDate = options.DateStart.HasValue;
            bool hasEndDate = options.DateEnd.HasValue;
            if (!hasStartDate && !hasEndDate)
                return query;
            else if (hasStartDate && hasEndDate && (options.DateStart.Value > options.DateEnd.Value))
                return query;
            else
            {
                string propertyName = options.ArticleType.Value == ArticleType.Highlighted
                    ? "HighlightedAt"
                    : options.ArticleType.Value == ArticleType.Published
                        ? "PublishedAt"
                        : "CreatedAt";

                if(hasStartDate)
                {
                    query = query.Where(a => EF.Property<DateTime>(a, propertyName) >= options.DateStart.Value);
                }
                if(hasEndDate)
                {
                    query = query.Where(a => EF.Property<DateTime>(a, propertyName) <= options.DateEnd.Value);
                }
            }

            return query;
        }

        private IQueryable<Article> RestrictSearch(IQueryable<Article> query, ArticleServiceOptions options)
        {

            string filteredSearchString = options.SearchString.Trim().ToLower();
            if (filteredSearchString.Length > 0)
            {
                bool isName = options.SearchType.Value.HasFlag(ArticleSearchType.Name);
                bool isLead = options.SearchType.Value.HasFlag(ArticleSearchType.Lead);
                bool isContent = options.SearchType.Value.HasFlag(ArticleSearchType.Content);
                if (isName || isLead || isContent)
                {
                    query = query.Where(a =>
                        (isName && a.Name.ToLower().Contains(filteredSearchString)) ||
                        (isLead && a.Lead.ToLower().Contains(filteredSearchString)) ||
                        (isContent && a.Content.ToLower().Contains(filteredSearchString)));
                }
            }

            return query;
        }

        #endregion


        #region Image

        public List<ArticleImage> GetArticleImages(string searchString = null)
        {
            string filteredSearchString = searchString.Trim().ToLower();

            IQueryable<ArticleImage> query = _context.ArticleImages;

            if (filteredSearchString.Length > 0)
            {
                query = query.Where(a => a.Name.ToLower().Contains(filteredSearchString));
            }

            return query.OrderBy(a => a.Id).ToList();
        }

        public ArticleImage GetArticleImageById(int id)
        {
            return _context.ArticleImages.FirstOrDefault(a => a.Id == id);
        }

        public List<ArticleImage> GetArticleImagesByArticle(Article article)
        {
            return _context.ArticleImages.Where(a => a.Article == article).ToList();
        }

        public List<ArticleImage> GetArticleImagesByArticle(int articleId)
        {
            return _context.ArticleImages.Where(a => a.Article.Id == articleId).ToList();
        }

        public bool CreateArticleImage(ArticleImage articleImage)
        {
            try
            {
                _context.Add(articleImage);
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }

        public NewsUpdateResult UpdateArticleImage(ArticleImage articleImage)
        {
            try
            {
                _context.Update(articleImage);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NewsUpdateResult.ConcurrencyError;
            }
            catch (DbUpdateException)
            {
                return NewsUpdateResult.DbError;
            }

            return NewsUpdateResult.Success;
        }

        public bool ArticleImageExists(int id)
        {
            return _context.ArticleImages.Any(e => e.Id == id);
        }

        public bool DeleteArticleImage(int id)
        {
            var articleImage = _context.ArticleImages.Find(id);
            if (articleImage == null)
                return false;

            try
            {
                _context.ArticleImages.Remove(articleImage);
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }
        #endregion

    }
}
