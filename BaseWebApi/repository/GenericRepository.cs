using BaseWebApi.dbFactory;
using BaseWebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace BaseWebApi.repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        //private northwindEntities dbContext;
        private SuitrohDBEntities dbContext;

        protected IDbFactory DbFactory
        {
            get;
            private set;
        }

        //protected northwindEntities DbContext
        protected SuitrohDBEntities DbContext
        {
            get { return dbContext ?? (dbContext = DbFactory.Init()); }
        }

        public GenericRepository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }

        public T GetById(Guid id)
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

        public IQueryable<T> GetAll(params Expression<Func<T, object>>[] includeExpressions)
        {
            IDbSet<T> dbSet = DbContext.Set<T>();

            //IQueryable<T> query = null;
            IQueryable<T> query = dbSet.Include(includeExpressions[0]);
            foreach (var includeExpression in includeExpressions.Skip(1))
            {
                //query = dbSet.Include(includeExpression);
                query = query.Include(includeExpression);
            }

            return query ?? dbSet;
            //return DbContext.Set<T>();            
        }

        public T Add(T entity)
        {
            DbContext.Set<T>().Add(entity);
            Save();
            return entity;
        }

        public T Update(T entity)
        {           
            DbContext.Entry(entity).State = EntityState.Modified;
            Save();
            return entity;
        }

        public void Remove(T entity)
        {
            DbContext.Set<T>().Remove(entity);
        }

        public void Save()
        {
            try
            {
                DbContext.SaveChanges();                
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        /*System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        --- > you just put the log to know the errors*/
                    }
                }
            }
        }

        /*public IQueryable<T> Include(params Expression<Func<T, object>>[] includeExpressions)
        {
            IDbSet<T> dbSet = DbContext.Set<T>();

            IQueryable<T> query = null;
            foreach (var includeExpression in includeExpressions)
            {
                query = dbSet.Include(includeExpression);
            }

            return query ?? dbSet;
        }*/

    }
}