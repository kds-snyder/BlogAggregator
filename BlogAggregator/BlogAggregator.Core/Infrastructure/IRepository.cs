using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BlogAggregator.Core.Infrastructure
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity Add(TEntity entity);

        bool Any(Expression<Func<TEntity, bool>> condition);

        int Count();

        int Count(Expression<Func<TEntity, bool>> where);

        void Delete(TEntity entity);

        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> where);

        TEntity GetByID(params object[] id);

        IQueryable<TEntity> GetAll();

        TEntity Update(TEntity entity);

        IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> where);

    }
}
