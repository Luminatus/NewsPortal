using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NewsPortal.Persistence
{
    public class NewsContext : IdentityDbContext<Editor, IdentityRole<int>, int>
    {
        public NewsContext(DbContextOptions<NewsContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Editor>().ToTable("Editors");
        }

        // Add a DbSet for each entity type that you want to include in your model.
        public DbSet<Article> Articles { get; set; }

        public DbSet<ArticleImage> ArticleImages { get; set; }
    }
}