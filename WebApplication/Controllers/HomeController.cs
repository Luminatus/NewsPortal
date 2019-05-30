using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewsPortal.Website.Models;
using NewsPortal.Persistence;
using NewsPortal.Website.Services;

namespace NewsPortal.Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly NewsService _newsService;
        private const int FRONTPAGE_LIMIT = 10;
        private const int ARCHIVE_LIMIT = 20;
        private const int FRONTPAGE_HIGHLIGHTED_LIMIT = 1;



        public HomeController(NewsService newsService)
        {
            _newsService = newsService;
        }
        public IActionResult Index()
        {
            ArticleServiceOptions options = new ArticleServiceOptions
            {                
                Limit = FRONTPAGE_LIMIT,
                ArticleType = ArticleType.Published,
                IncludeAuthor = true
            };

            ArticleServiceOptions highlightedOptions = new ArticleServiceOptions
            {
                Limit = 10,
                ArticleType = ArticleType.Highlighted,
                IncludeAuthor = true,
                IncludeImages = true
            };


            List<Article> articles = _newsService.GetArticles(options);
            //Article leadingArticle = _newsService.GetArticles(highlightedOptions).FirstOrDefault();
            List<Article> leadingArticles = _newsService.GetArticles(highlightedOptions);

            MainPageViewModel viewModel = new MainPageViewModel();

            foreach(Article article in articles)
            {
                viewModel.Articles.Add(new ArticleViewModel(article));
            }

            foreach (Article article in leadingArticles)
            {
                viewModel.LeadingArticles.Add(new ArticleViewModel(article));
            }

            return View(viewModel);
        }

        [HttpGet("article/{id}")]
        public IActionResult Detail(int id)
        {
            Article article = _newsService.GetArticleById(id);
            if(article != null)
            {
                ArticleViewModel viewModel = new ArticleViewModel(article);
                return View(viewModel);
            }
            else
            {
                return NotFound();
            }

        }
        [HttpGet("archive/{page?}")]
        public IActionResult Archive(int page, [FromQuery]SearchModel data)
        {
            if (page == 0)
                page = 1;

            ArticleSearchType searchType = 0;

            if (data.SearchTitle)
                searchType = searchType | ArticleSearchType.Name;
            if (data.SearchLead)
                searchType = searchType | ArticleSearchType.Lead;
            if (data.SearchContent)
                searchType = searchType | ArticleSearchType.Content;



            ArticleServiceOptions options = new ArticleServiceOptions
            {
                Offset = (page - 1) * ARCHIVE_LIMIT,
                Limit = ARCHIVE_LIMIT,
                ArticleType = ArticleType.Published,
                IncludeAuthor = true,
                SearchType = searchType,
                SearchString = data.Search,
                DateStart = data.DateStart,
                DateEnd = data.DateEnd
            };

            if(options.DateStart.HasValue && options.DateEnd.HasValue && options.DateStart.Value > options.DateEnd.Value)
            {
                ModelState.AddModelError("DateStart", "Kezdődátum nem lehet nagyobb a végdátumnál");
                ModelState.AddModelError("DateEnd", "Kezdődátum nem lehet nagyobb a végdátumnál");

                var vm = new ArchivePageViewModel
                {
                    Search = data,
                    Articles = { }
                };
                return View(vm);
            }

            if(options.DateEnd.HasValue)
            {
                options.DateEnd = options.DateEnd.Value.AddDays(1).Date; //Add one day to include DateEnd in search
            }
            if(options.DateStart.HasValue)
            {
                options.DateStart = options.DateStart.Value.Date; //Set DateTime to midnight;
            }


            List<Article> articles = _newsService.GetArticles(options);
            int count = _newsService.GetArticleCount(options);

            int pageCount = options.Limit.Value > 0 ? ((count - 1) / options.Limit.Value) + 1 : 1;

            ArchivePageViewModel viewModel = new ArchivePageViewModel
            {
                Search = new SearchModel
                {
                    Limit = options.Limit ?? 0,
                    Page = page,
                    PageCount = pageCount,
                    Search = options.SearchString ?? "",
                    DateStart = options.DateStart,
                    DateEnd = data.DateEnd,
                },
                Articles = articles.Select(a => new ArticleViewModel(a)).ToList()
            };

            return View(viewModel);
        }
    }
}
