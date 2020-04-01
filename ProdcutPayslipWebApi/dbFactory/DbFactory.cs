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
        NominalDataEntities dbContext;
        //public northwindEntities Init()
        public NominalDataEntities Init()
        {
            return dbContext ?? (dbContext = new NominalDataEntities());
            //return dbContext ?? (dbContext = new northwindEntities());
        }
    }
}