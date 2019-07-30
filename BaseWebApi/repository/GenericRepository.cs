using BaseWebApi.dbFactory;
using BaseWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public IQueryable<T> GetAll()
        {
            return DbContext.Set<T>();
        }
    }
}