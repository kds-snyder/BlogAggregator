using BlogAggregator.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogAggregator.Data.Infrastructure
{
    public class DatabaseFactory : Disposable, IDatabaseFactory
    {
        private readonly BlogAggregatorDbContext _dataContext;

        public BlogAggregatorDbContext GetDataContext()
        {
            return _dataContext ?? new BlogAggregatorDbContext();
        }

        public DatabaseFactory()
        {
            _dataContext = new BlogAggregatorDbContext();
        }

        protected override void DisposeCore()
        {
            if (_dataContext != null)
            {
                _dataContext.Dispose();
            }
        }

    }
}
