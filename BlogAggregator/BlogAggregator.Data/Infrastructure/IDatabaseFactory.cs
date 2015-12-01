using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogAggregator.Data.Infrastructure
{
    public interface IDatabaseFactory : IDisposable
    {
        BlogAggregatorDbContext GetDataContext();
    }
}
