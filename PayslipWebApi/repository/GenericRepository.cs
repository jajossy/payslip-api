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
        private NominalDataEntities dbContext;

        protected IDbFactory DbFactory
        {
            get;
            private set;
        }

        //protected northwindEntities DbContext
        protected NominalDataEntities DbContext
        {
            get { return dbContext ?? (dbContext = DbFactory.Init()); }
        }

        public GenericRepository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }

        public GenericRepository()
        {
            this.dbContext = new NominalDataEntities();

        }

        public T GetById(Guid id)
        {
            return DbContext.Set<T>().Find(id);
        }

        /*public T GetById(Guid id, params Expression<Func<T, object>>[] includeExpressions)
        {
            IDbSet<T> dbSet = DbContext.Set<T>();
            //T dbSet = DbContext.Set<T>().Find(id);

            //IQueryable<T> query = null;
            if (includeExpressions.Count() > 0)
            {
                IQueryable<T> query = dbSet.Include(includeExpressions[0]);
                //T query = dbSet.Include(includeExpressions[0]);
                foreach (var includeExpression in includeExpressions.Skip(1))
                {
                    //query = dbSet.Include(includeExpression);
                    query = query.Include(includeExpression);
                }                
                return query.FirstOrDefault() ?? dbSet.Find(id);
            }

            //return DbContext.Set<T>();
            return DbContext.Set<T>().Find(id);
        }*/

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
            if(includeExpressions.Count() > 0)
            {
                IQueryable<T> query = dbSet.Include(includeExpressions[0]);
                foreach (var includeExpression in includeExpressions.Skip(1))
                {
                    //query = dbSet.Include(includeExpression);
                    query = query.Include(includeExpression);
                }

                return query ?? dbSet;
            }
            
            return DbContext.Set<T>();            
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

        public T Remove(T entity)
        {
            DbContext.Set<T>().Remove(entity);
            Save();
            return entity;
        }

        public T RemoveEntity(T entity)
        {
            if (entity != null)
            {
                var attachedEntry = DbContext.Entry(entity);
                attachedEntry.State = EntityState.Deleted;
            }
            Save();
            return entity;
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

        /*static bool IsValid(LambdaExpression expression)
        {
            // Expression is in the form of parameter => something
            // Body is the 'something' part
            var body = expression.Body;

            // MemberExpression are like p.Name, that's a valid body
            if (body is MemberExpression)
                return true;

            // MethodCallExpression are like p.Select(...) or p.Where(...) or p.DoSomething(...)
            var methodCallExpression = body as MethodCallExpression;

            // If it's not a methodcall, it can't be a select, so it's invalid
            if (methodCallExpression == null)
                return false;

            // Method contains the actual MethodInfo
            var method = methodCallExpression.Method;

            // Select is a generic method, so if it's not generic, it can't be valid
            if (!method.IsGenericMethod)
                return false;

            // Get the actual, parameterless methoddefinition of Enumerable.Select
            // NOTE: This is ugly as hell, but AFAIK there's no better way 
            // just query for the method whose name is 'Select' and has two parameters where the second one has two generic arguments (that's the Func argument)
            var selectMethod = typeof(Enumerable).GetMethods()
                              .Single(m => m.Name == nameof(Enumerable.Select) && m.GetParameters()[1].ParameterType.GetGenericArguments().Count() == 2);

            // If the method in the methodinfo is not the Select definition, it's not valid
            if (method.GetGenericMethodDefinition() != selectMethod)
                return false;

            // Otherwise the methodcall is in the form of p.Select(p=>'something else')
            // innerExpr gets the p=>'something else' part
            var innerExpr = methodCallExpression.Arguments[1];

            // If the expression really is a lambda expression, then recursively check the p=>'something else' part
            if (innerExpr is LambdaExpression lambda)
            {
                return IsValid(lambda);
            }
            else
            {
                // Otherwise it's invalid
                // NOTE: this is just in case, I'm not even sure if you can achieve this with regular C# code at compilation time
                return false;
            }
        }*/

    }
}