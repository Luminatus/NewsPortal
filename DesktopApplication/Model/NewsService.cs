using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NewsPortal.Persistence;
using NewsPortal.Persistence.DTO;

namespace NewsPortal.DesktopApplication.Model
{
    public class NewsService : INewsService
    {
        private readonly HttpClient _client;

        private bool _isUserLoggedIn;
        public bool IsUserLoggedIn => _isUserLoggedIn;

        public NewsService(string baseAddress)
        {
            _isUserLoggedIn = false;
            _client = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };
        }
        public async Task<IEnumerable<String>> Test()
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync("api/values");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<IEnumerable<String>>();
                }

                throw new NetworkException("Service returned response: " + response.StatusCode);
            }
            catch (Exception e)
            {
                return null;
            }

        }


        public async Task<ArticleListDTO> LoadArticlesAsync(int page = 1)
        {
            HttpResponseMessage response = await _client.GetAsync("api/Articles?page="+page);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<ArticleListDTO>();
            }

            throw new NetworkException("Service returned response: " + response.StatusCode);
        }

        public async Task<ArticleClientData> GetArticleAsync(int id)
        {
            HttpResponseMessage response = await _client.GetAsync("api/Articles/" + id);

            if (response.IsSuccessStatusCode)
            {
                ArticleDTO article = await response.Content.ReadAsAsync<ArticleDTO>();

                ArticleClientData clientArticle = new ArticleClientData(article);
                foreach (var image in clientArticle.Images)
                {
                    image.IsUploaded = true;
                }

                clientArticle.IsUploaded = true;

                return clientArticle;
            }

            throw new NetworkException("Service returned response: " + response.StatusCode);

        }

        public async Task<bool> LoginAsync(string name, string password)
        {
            LoginDTO user = new LoginDTO(name, password);
            //HttpResponseMessage response = await _client.PostAsync("api/Account/Login",
            //    new StringContent(JsonConvert.SerializeObject(user),
            //        Encoding.UTF8,
            //        "application/json"));
            HttpResponseMessage response = await _client.PostAsJsonAsync("api/Account/Login", user);

            if (response.IsSuccessStatusCode)
            {
                _isUserLoggedIn = true;
                return true;
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return false;
            }

            throw new NetworkException("Service returned response: " + response.StatusCode);
        }

        public async Task<bool> LogoutAsync()
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync("api/Account/Signout", "");

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            throw new NetworkException("Service returned response: " + response.StatusCode);
        }

        public async Task<EditorDTO> GetUserAsync()
        {
            if(!IsUserLoggedIn)
            {
                return null;
            }

            HttpResponseMessage response = await _client.GetAsync("api/Account");
            if(response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<EditorDTO>();
            }
            else
            {
                throw new NetworkException("Service returned response: " + response.StatusCode);
            }
        }

        public async Task<Boolean> SaveArticleAsync(ArticleClientData article)
        {
            var errors = ValidateArticle(article);
            if(errors.Count > 0)
            {
                string errorsString = "";
                foreach(string error in errors)
                {
                    errorsString += String.Format("- {0}\n", error);
                }
                throw new NetworkException(errorsString);
            }

            List<ImageClientData> images = new List<ImageClientData>(article.Images);
            images = images.FindAll(elem => !elem.IsDeleted);
            List<ImageClientData> newImages = images.FindAll(elem => elem.IsUploaded == false);

            if (!article.IsUploaded)
            {
                HttpResponseMessage putResponse = await _client.PutAsJsonAsync("api/Articles", (ArticleDTO)article);
                if(putResponse.IsSuccessStatusCode)
                {
                    ArticleDTO responseData = await putResponse.Content.ReadAsAsync<ArticleDTO>();
                    article.Id = responseData.Id;
                    article.CreatedAt = responseData.CreatedAt;                    
                    article.IsUploaded = true;
                }
                else
                {
                    throw new NetworkException("Service returned response: " + putResponse.StatusCode);
                }
            }

            try
            {
                foreach (ImageClientData image in newImages)
                {
                    await UploadImageAsync(image,article.Id);
                }
            }
            catch(Exception ex)
            {
                throw ex;    
            }
            finally
            {
                article.Images = images;
            }

            /*ArticleDTO data = new ArticleDTO()
            {
                Id = article.Id,
                Name = article.Name,
                Content = article.Content,
                Lead = article.Lead,
                Images = article.Images,
            };*/
            
            HttpResponseMessage response = await _client.PostAsJsonAsync("api/Articles/" + article.Id, (ArticleDTO)article);
            if(response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                throw new NetworkException("Service returned response: " + response.StatusCode);
            }

        }
        public async Task<Boolean> UploadImageAsync(ImageClientData image, int id)
        {
            HttpResponseMessage response = await _client.PutAsJsonAsync("api/Articles/"+id+"/Image", (ImageDTO)image);
            if(response.IsSuccessStatusCode)
            {
                ImageDTO responseData = await response.Content.ReadAsAsync<ImageDTO>();
                image.IsUploaded = true;
                image.Id = responseData.Id;
                image.Name = responseData.Name;
                image.Base64 = responseData.Base64;
                return true;
            }
            else
            {
                throw new NetworkException("Service returned response: " + response.StatusCode);
            }
        }

        public async Task<Boolean> HighlightArticleAsync(int id)
        {
            HttpResponseMessage response = await _client.PostAsync("api/Articles/" + id + "/Highlight",null);
            if(response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                throw new NetworkException("Service returned response: " + response.StatusCode);
            }
        }

        public async Task<Boolean> PublishArticleAsync(int id)
        {
            HttpResponseMessage response = await _client.PostAsync("api/Articles/" + id + "/Publish", null);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                throw new NetworkException("Service returned response: " + response.StatusCode);
            }
        }

        public async Task<Boolean> UnPublishArticleAsync(int id)
        {
            HttpResponseMessage response = await _client.PostAsync("api/Articles/" + id + "/UnPublish", null);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                throw new NetworkException("Service returned response: " + response.StatusCode);
            }
        }

        public async Task<Boolean> DeleteArticleAsync(int id)
        {
            HttpResponseMessage response = await _client.DeleteAsync("api/Articles/" + id);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                throw new NetworkException("Service returned response: " + response.StatusCode);
            }
        }

        private IList<string> ValidateArticle(ArticleDTO article)
        {
            var errors = new List<string>();
            if (article.Name.Length == 0)
                errors.Add("Cikk címe nem lehet üres");
            else if (article.Name.Length > 100)
                errors.Add("Cikk címe nem lehet 100 karakternél hosszabb");

            if (article.Lead.Length == 0)
                errors.Add("Cikk bevezetője nem lehet üres");
            else if (article.Lead.Length > 1000)
                errors.Add("Cikk bevezetője nem lehet 1000 karakternél hosszabb");

            if (article.Content.Length == 0)
                errors.Add("Cikk tartalma nem lehet üres");

            return errors;
                
        }
    }
}