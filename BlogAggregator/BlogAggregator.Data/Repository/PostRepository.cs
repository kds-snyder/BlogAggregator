using BlogAggregator.Core.Domain;
using BlogAggregator.Core.Repository;
using BlogAggregator.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogAggregator.Data.Repository
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        public PostRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {

        }

        public override IQueryable<Post> GetAll()
        {
            return DbSet;
        }
    }
}
