using BlogAggregator.Core.Infrastructure;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace BlogAggregator.Data.Infrastructure
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private BlogAggregatorDbContext _dataContext;
        protected BlogAggregatorDbContext DataContext
        {
            get
            {
                return _dataContext ?? (_dataContext = DatabaseFactory.GetDataContext());
            }
            set
            {
                _dataContext = value;
            }
        }
        protected IDatabaseFactory DatabaseFactory { get; private set; }

        protected IDbSet<TEntity> DbSet { get; set; }

        protected Repository(IDatabaseFactory databaseFactory)
        {
            DatabaseFactory = databaseFactory;
            DbSet = DataContext.Set<TEntity>();
        }

        public virtual TEntity Add(TEntity entity)
        {
            return DbSet.Add(entity);
        }

        public bool Any(Expression<Func<TEntity, bool>> condition)
        {
            return DbSet.Any(condition);
        }

        public virtual int Count()
        {
            return DbSet.Count();
        }

        public virtual int Count(Expression<Func<TEntity, bool>> where)
        {
            return DbSet.Count(where);
        }

        public virtual void Delete (TEntity entity)
        {
            DbSet.Remove(entity);
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> where)
        {           
            return DbSet.FirstOrDefault(where);
        }

        public virtual TEntity GetByID (params object[] id)
        {
            return DbSet.Find(id);
        }
        
        public virtual IQueryable<TEntity> GetAll ()
        {
            return DbSet;
        }
       
        public virtual TEntity Update(TEntity entity)
        {
            var t = DbSet.Attach(entity);
            DataContext.Entry(entity).State = EntityState.Modified;
            return t;
        }

        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> where)
        {
            return DbSet.Where(where);
        }

    }
}
