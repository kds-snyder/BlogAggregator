using BlogAggregator.Core.Domain;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogAggregator.Core.Infrastructure
{
    public class BlogAggregatorDbContext : IdentityDbContext
    {

        public BlogAggregatorDbContext() : base("BlogAggregator")
        {
        }

        public IDbSet<Blog> Blogs { get; set; }
        public IDbSet<Post> Posts { get; set; }

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
