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
    public class CountryController : ApiController
    {
        private readonly IGenericRepository<Country> _countryRepository;

        public CountryController(IGenericRepository<Country> countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public IQueryable<Country> Get()
        {
            return _countryRepository.GetAll();
        }
    }
}
