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

        public BlogAggregatorDbContext() : base("BlogAggregator_oauth")
        {
            var ensureDLLIsCopied =
                System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }

        public IDbSet<Blog> Blogs { get; set; }
        public IDbSet<Post> Posts { get; set; }
           
        public IDbSet<ExternalLogin> ExternalLogins { get; set; }
        public IDbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>().HasKey(b => b.BlogID);
            modelBuilder.Entity<Blog>().HasMany(b => b.Posts)
                                         .WithRequired(p => p.Blog)
                                         .HasForeignKey(p => p.BlogID);

            modelBuilder.Entity<Post>().HasKey(p => p.PostID);

            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<User>().HasMany(b => b.ExternalLogins)
                                       .WithRequired(el => el.User)
                                       .HasForeignKey(el => el.UserID);

            modelBuilder.Entity<ExternalLogin>().HasKey(u => u.ExternalLoginID);

            base.OnModelCreating(modelBuilder);
        }
    }
}
