using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace BaseWebApi.repository
{
    public interface IGenericRepository<T> where T : class
    {
        T GetById(Guid id);
        IQueryable<T> GetAll(params Expression<Func<T, object>>[] includeExpressions);
        T Add(T entity);
        T Update(T entity);
        T Remove(T entity);
        T RemoveEntity(T entity);

        //IQueryable<T> Include(params Expression<Func<T, object>>[] includeExpressions);
    }
}