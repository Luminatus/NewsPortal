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
        Task<ArticleListDTO> LoadArticlesAsync(int page);
        Task<ArticleClientData> GetArticleAsync(int id);
        Task<Boolean> SaveArticleAsync(ArticleClientData article);
        Task<Boolean> UploadImageAsync(ImageClientData image, int id);
        Task<bool> HighlightArticleAsync(int id);
        Task<bool> DeleteArticleAsync(int id);
        Task<bool> PublishArticleAsync(int id);
        Task<bool> UnPublishArticleAsync(int id);
        Task<bool> LoginAsync(string name, string password);
        Task<bool> LogoutAsync();
        Task<EditorDTO> GetUserAsync();
        Task<IEnumerable<String>> Test();
    }
}