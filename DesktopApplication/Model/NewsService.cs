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
            catch(Exception e)
            {
                int col = 13;
                return null;
            }

        }


        public async Task<IEnumerable<Article>> LoadArticlesAsync()
        {
            HttpResponseMessage response = await _client.GetAsync("api/Lists/");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IEnumerable<Article>>();
            }

            throw new NetworkException("Service returned response: " + response.StatusCode);
        }

        public async Task<bool> LoginAsync(string name, string password)
        {
            EditorDTO user = new EditorDTO(name, password);            
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
    }
}