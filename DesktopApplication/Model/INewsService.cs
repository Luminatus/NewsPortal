using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewsPortal.Persistence;

namespace NewsPortal.DesktopApplication.Model
{
    public interface INewsService
    {
        bool IsUserLoggedIn { get; }
        Task<IEnumerable<Article>> LoadArticlesAsync();
        Task<bool> LoginAsync(string name, string password);
        Task<bool> LogoutAsync();
        Task<IEnumerable<String>> Test();
    }
}