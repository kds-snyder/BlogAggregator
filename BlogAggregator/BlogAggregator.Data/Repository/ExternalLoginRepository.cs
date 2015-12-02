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
    public class ExternalLoginRepository : Repository<ExternalLogin>, IExternalLoginRepository
    {
        public ExternalLoginRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {

        }
    }
}
