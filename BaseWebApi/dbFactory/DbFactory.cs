using BaseWebApi.dbFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaseWebApi.Models
{
    public class DbFactory : IDbFactory
    {
        //northwindEntities dbContext;
        SuitrohDBEntities dbContext;
        //public northwindEntities Init()
        public SuitrohDBEntities Init()
        {
            return dbContext ?? (dbContext = new SuitrohDBEntities());
            //return dbContext ?? (dbContext = new northwindEntities());
        }
    }
}