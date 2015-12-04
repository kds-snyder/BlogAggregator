using BlogAggregator.Core.Domain;
using BlogAggregator.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogAggregator.Core.Repository
{
    public interface IUserRepository : IRepository<User>
    {
    }
}
