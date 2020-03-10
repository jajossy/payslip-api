using BaseWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaseWebApi.dbFactory
{
    public interface IDbFactory
    {
        //northwindEntities Init();
        NominalDataEntities Init();
    }
}