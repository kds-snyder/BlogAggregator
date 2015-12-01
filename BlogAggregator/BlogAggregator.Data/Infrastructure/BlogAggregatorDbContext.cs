using BlogAggregator.Core.Domain;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogAggregator.Data.Infrastructure
{
    public class BlogAggregatorDbContext : DbContext
    {

        public BlogAggregatorDbContext() : base("BlogAggregator_repo")
        {
        }

        public IDbSet<Blog> Blogs { get; set; }
        public IDbSet<Post> Posts { get; set; }
        public IDbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>().HasKey(b => b.BlogID);
            modelBuilder.Entity<Blog>().HasMany(b => b.Posts)
                                         .WithRequired(p => p.Blog)
                                         .HasForeignKey(p => p.BlogID);

            modelBuilder.Entity<Post>().HasKey(p => p.PostID);

            base.OnModelCreating(modelBuilder);
        }
    }
}
