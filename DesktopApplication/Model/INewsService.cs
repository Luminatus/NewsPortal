using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewsPortal.Persistence;
using NewsPortal.Persistence.DTO;

namespace NewsPortal.DesktopApplication.Model
{
    public interface INewsService
    {
        bool IsUserLoggedIn { get; }
        Task<ArticleListDTO> LoadArticlesAsync();
        Task<ArticleDTO> GetArticleAsync(int id);
        Task<bool> LoginAsync(string name, string password);
        Task<bool> LogoutAsync();
        Task<IEnumerable<String>> Test();
    }
}