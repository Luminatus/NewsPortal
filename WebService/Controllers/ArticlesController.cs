using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Persistence;
using NewsPortal.Persistence.DTO;
using System.ComponentModel.DataAnnotations;
using NewsPortal.WebService.Models.DataStructures;
using Microsoft.AspNetCore.Identity;

namespace WebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly NewsContext _context;
        private readonly UserManager<Editor> _userManager;
        private const int LIST_LIMIT = 20;

        public class SearchParameters
        {
            [FromQuery(Name = "page")]
            public int Page { get; set; }

            [FromQuery(Name = "limit")]
            public int Limit { get; set; }

            [FromQuery(Name = "search")]
            public string Search { get; set; }

            public int PageCount { get; set; }

            [DataType(DataType.Date)]
            [FromQuery(Name = "date_start")]
            [DisplayFormat(DataFormatString = "{yyyy-MM-dd}")]
            public DateTime? DateStart { get; set; }

            [DataType(DataType.Date)]
            [FromQuery(Name = "date_end")]
            [DisplayFormat(DataFormatString = "{yyyy-MM-dd}")]
            public DateTime? DateEnd { get; set; }

            [FromQuery(Name = "search_title")]
            public bool SearchTitle { get; set; }

            [FromQuery(Name = "search_lead")]
            public bool SearchLead { get; set; }

            [FromQuery(Name = "search_content")]
            public bool SearchContent { get; set; }
            
        }

        public ArticlesController(NewsContext context, UserManager<Editor> manager)
        {
            _context = context;
            _userManager = manager;
        }

        // GET: api/Articles/
        [HttpGet]
        public async Task<ArticleListDTO> GetArticles(int page, [FromQuery]SearchParameters parameters)
        {
            if (page == 0)
                page = 1;

            ArticleSearchType searchType = 0;

            if (parameters.SearchTitle)
                searchType = searchType | ArticleSearchType.Name;
            if (parameters.SearchLead)
                searchType = searchType | ArticleSearchType.Lead;
            if (parameters.SearchContent)
                searchType = searchType | ArticleSearchType.Content;


            ArticleApiOptions options = new ArticleApiOptions
            {
                Offset = (page - 1) * LIST_LIMIT,
                Limit = LIST_LIMIT,
                ArticleType = ArticleType.Published,
                IncludeAuthor = true,
                SearchType = searchType,
                SearchString = parameters.Search,
                DateStart = parameters.DateStart,
                DateEnd = parameters.DateEnd
            };

            if (options.DateEnd.HasValue)
            {
                options.DateEnd = options.DateEnd.Value.AddDays(1).Date; //Add one day to include DateEnd in search
            }
            if (options.DateStart.HasValue)
            {
                options.DateStart = options.DateStart.Value.Date; //Set DateTime to midnight;
            }


            //_newsService.GetArticles(options);

            IQueryable<Article> query = _context.Articles;

            query = RestrictArticleType(query, options.ArticleType.Value);
            query = RestrictDate(query, options);
            query = RestrictSearch(query, options);

            if (options.IncludeAuthor.HasValue && options.IncludeAuthor.Value)
            {
                query = query.Include(a => a.Author);
            }


            int count = query.Count();

            if (options.Offset.HasValue && options.Offset.Value > 0)
            {
                query = query.Skip(options.Offset.Value);
            }

            if (options.Limit.HasValue && options.Limit.Value > 0)
            {
                query = query.Take(options.Limit.Value);
            }

            int pageCount = options.Limit.Value > 0 ? ((count - 1) / options.Limit.Value) + 1 : 1;

            List<ArticleListElemDTO> list = query.Select<Article, ArticleListElemDTO>(x =>  new ArticleListElemDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    Lead = x.Lead,
                    ImageCount = x.Images.Count(),
                    Author = x.Author.Name,
                    PublishedAt = x.PublishedAt,
                    IsPublished = x.IsPublished,
                    IsHighlighted = x.IsHighlighted,
                    HighlightedAt = x.HighlightedAt,
                    CreatedAt = x.CreatedAt
                }
            ).ToList();

            ArticleListDTO result = new ArticleListDTO {
                Limit = options.Limit.Value,
                Page = page,
                Count = count,
                PageCount = pageCount,
                Articles = list
            };

            return result;
        }

        // GET: api/Articles/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticle([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var article = await _context.Articles.Include("Author").Include("Images").FirstOrDefaultAsync(x => x.Id == id);

            if (article == null)
            {
                return NotFound();
            }

            ArticleDTO articleDTO = new ArticleDTO
            {
                Id = article.Id,
                Name = article.Name,
                Lead = article.Lead,
                Content = article.Content,
                Author = article.Author.Name,
                CreatedAt = article.CreatedAt,
                HighlightedAt = article.HighlightedAt,
                PublishedAt = article.PublishedAt,
                IsPublished = article.IsPublished,
                IsHighlighted = article.IsHighlighted,
                Images = article.Images.Select(x => { return new ImageDTO { Id = x.Id, Base64 = Convert.ToBase64String(x.Image) }; })
            };

            return Ok(articleDTO);
        }

        // PUT: api/Articles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticle([FromRoute] int id, [FromBody] Article article)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != article.Id)
            {
                return BadRequest();
            }

            _context.Entry(article).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Articles
        [HttpPost]
        public async Task<IActionResult> PostArticle([FromBody] Article article)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetArticle", new { id = article.Id }, article);
        }

        // DELETE: api/Articles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();

            return Ok(article);
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.Id == id);
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

        private IQueryable<Article> RestrictDate(IQueryable<Article> query, ArticleApiOptions options)
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

                if (hasStartDate)
                {
                    query = query.Where(a => EF.Property<DateTime>(a, propertyName) >= options.DateStart.Value);
                }
                if (hasEndDate)
                {
                    query = query.Where(a => EF.Property<DateTime>(a, propertyName) <= options.DateEnd.Value);
                }
            }

            return query;
        }

        private IQueryable<Article> RestrictSearch(IQueryable<Article> query, ArticleApiOptions options)
        {
            string filteredSearchString = options.SearchString != null ? options.SearchString.Trim().ToLower() : "";
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
    }
}