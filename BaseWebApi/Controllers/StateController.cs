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
    public class StateController : ApiController
    {
        private readonly IGenericRepository<State> _stateRepository;

        public StateController(IGenericRepository<State> stateRepository)
        {
            _stateRepository = stateRepository;
        }

        [HttpGet]
        public IQueryable<State> Get(int id)
        {
            return _stateRepository.GetAll().Where(x => x.CountryId == id);
        }        

    }
}
