using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace NewsPortal.Persistence
{
    public static class DbInitializer
    {

        private static NewsContext _context;
        private static UserManager<Editor> _userManager;
        private static RoleManager<IdentityRole<int>> _roleManager;
        
        public static void Initialize(NewsContext context, UserManager<Editor> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;

            // Adatbázis létrehozása, amennyiben nem létezik
            _context.Database.EnsureCreated();

            // Listák keresése
            if (_context.Articles.Any())
            {
                return; // Az adatbázis már inicializálva van.
            }

            if (!_context.Users.Any())
            {
                SeedUsers();
            }
            if (!_context.Articles.Any())
            {
                SeedArticles();
            }
        }

        /// <summary>
		/// Adminisztrátorok inicializálása.
		/// </summary>
		private static bool SeedUsers()
        {
            var adminUser = new Editor
            {
                UserName = "admin",
                Name = "Adminisztrátor",
                Email = "admin@example.com",
                PhoneNumber = "+36123456789",
                Address = "Nevesincs utca 1."
            };
            var adminPassword = "qwertz";
            var adminRole = new IdentityRole<int>("administrator");

            var result1 = _userManager.CreateAsync(adminUser, adminPassword).Result;
            var result4 = _userManager.AddClaimAsync(adminUser, new System.Security.Claims.Claim("Id", adminUser.Id.ToString()));            
            var result2 = _roleManager.CreateAsync(adminRole).Result;
            var result3 = _userManager.AddToRoleAsync(adminUser, adminRole.Name).Result;

            return true;
        }

        private static bool SeedArticles()
        {
            Editor adminuser = _userManager.Users.FirstOrDefault<Editor>((x => x.UserName == "admin"));
            if (adminuser == null)
            {
                return false;
            }

            var random = new Random();

            var contentFileName = System.IO.Directory.GetFiles("../Persistence/InitData/Content").First();

            string articleContent = System.IO.File.ReadAllText(contentFileName);
            string articleLead = articleContent.Substring(0, articleContent.IndexOf(Environment.NewLine));

            DateTime now = DateTime.Now;

            var articles = new List<Article>();
            
            for(int i=0; i< 50; i++)
            {
                var randomDate = now.Date.AddDays(random.Next(-10, 11)).AddHours(random.Next(0, 24)).AddMinutes(random.Next(0, 60));
                articles.Add(new Article
                {
                    Name = "Test Article "+i.ToString(),
                    Lead = articleLead,
                    Content = articleContent,
                    Author = adminuser,
                    CreatedAt = randomDate,
                    UpdatedAt = randomDate,
                    PublishedAt = randomDate
                });
            }

            var leadingArticle = articles.First();
            leadingArticle.IsHighlighted = true;

            var file_names = System.IO.Directory.GetFiles("../Persistence/InitData/Images");

            foreach(string file_name in file_names)
            {
                var file = System.IO.File.ReadAllBytes(file_name);
                string name = System.IO.Path.GetFileName(file_name);
                leadingArticle.Images.Add( new ArticleImage { Image = file, Name = name });

                foreach(var article in articles.Except(new List<Article> { leadingArticle }))
                {
                    if( 0 < random.Next(3))
                        continue;

                    article.Images.Add(
                        new ArticleImage { Image = file, Name = name }
                    );
                }
            }

            foreach (var article in articles)
            {
                _context.Articles.Add(article);
            }
            _context.SaveChanges();

            return true;
        }
    }
}