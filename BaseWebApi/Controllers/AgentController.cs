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
    public class AgentController : ApiController
    {
        private readonly IGenericRepository<FieldAgent> _agentRepository;
        //private readonly IGenericRepository<User> _userRepository;

        public AgentController(IGenericRepository<FieldAgent> agentRepository)
        {
            _agentRepository = agentRepository;            
        }

        public IQueryable<FieldAgent> Get()
        {
            return _agentRepository.GetAll();
        }

        [HttpGet]
        public FieldAgent GetById(Guid id)
        {            
            var agent = _agentRepository.GetAll().Where(x => x.id == id).FirstOrDefault();            
            return agent;
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody]FieldAgent agent)
        {
            if (!ModelState.IsValid) return Request.CreateResponse(HttpStatusCode.BadRequest);
            agent.id = Guid.NewGuid();
            agent.DateCreated = DateTime.Now;
            agent.Status = true;
            // Save Field Agent data
            var savedEntity = _agentRepository.Add(agent);
            var response = Request.CreateResponse(HttpStatusCode.Created, savedEntity);

            return response;
        }

        [HttpPut]
        public void Put([FromBody]FieldAgent agent)
        {
            if (agent == null) throw new ArgumentNullException("agent");

            // update current stock data
            _agentRepository.Update(agent);
        }

        [HttpDelete]
        public void Delete(Guid id)
        {
            var agent = _agentRepository.GetById(id);

            if (agent == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            _agentRepository.Remove(agent);
        }
    }
}
