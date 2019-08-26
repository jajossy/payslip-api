using BaseWebApi.Models;
using BaseWebApi.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BaseWebApi.Controllers
{
    public class GenderController : ApiController
    {
        private readonly IGenericRepository<Gender> _genderRepository;

        public GenderController(IGenericRepository<Gender> genderRepository)
        {
            _genderRepository = genderRepository;
        }

        public IQueryable<Gender> Get()
        {
            return _genderRepository.GetAll();
        }
    }
}
