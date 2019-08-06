using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaseWebApi.repository
{
    public interface IGenericRepository<T> where T : class
    {
        T Get(int id);
        IQueryable<T> GetAll();
    }
}