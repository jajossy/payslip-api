using BaseWebApi.dbFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaseWebApi.Models
{
    public class DbFactory : IDbFactory
    {
        northwindEntities dbContext;
        public northwindEntities Init()
        {
            return dbContext ?? (dbContext = new northwindEntities());
        }
    }
}