using BlogAggregator.Core.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogAggregator.Core.Infrastructure
{
    public interface IBlogAggregatorDbContext : IDisposable
    {       
        IDbSet<Blog> Blogs { get; set; }
        IDbSet<Post> Posts { get; set; }

        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        int SaveChanges();
    }
}
