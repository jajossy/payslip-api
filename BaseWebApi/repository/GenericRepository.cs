using BaseWebApi.dbFactory;
using BaseWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace BaseWebApi.repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private northwindEntities dbContext;

        protected IDbFactory DbFactory
        {
            get;
            private set;
        }

        protected northwindEntities DbContext
        {
            get { return dbContext ?? (dbContext = DbFactory.Init()); }
        }

        public GenericRepository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }

        public T Get(int id)
        {
            return DbContext.Set<T>().Find(id);
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return DbContext.Set<T>().Where(predicate);
        }

        public T SingleOrDefault(Expression<Func<T, bool>> predicate)
        {
            return DbContext.Set<T>().SingleOrDefault(predicate);
        }

        public IQueryable<T> GetAll()
        {
            return DbContext.Set<T>();
        }

        public void Add(T entity)
        {
            DbContext.Set<T>().Add(entity);
        }

        public void Remove(T entity)
        {
            DbContext.Set<T>().Remove(entity);
        }
    }
}