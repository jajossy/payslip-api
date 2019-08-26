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
    public class CustomerController : ApiController
    {
        private readonly IGenericRepository<Customer> _customerRepository;

        public CustomerController(IGenericRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public IQueryable<Customer> Get()
        {
            return _customerRepository.GetAll();
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody]Customer customer)
        {
            if (!ModelState.IsValid) return Request.CreateResponse(HttpStatusCode.BadRequest);
            customer.id = Guid.NewGuid();
            customer.DateCreated = DateTime.Now;
            // Save Current Stock data
            var savedEntity = _customerRepository.Add(customer);            
            var response = Request.CreateResponse(HttpStatusCode.Created, savedEntity);

            return response;
        }

        [HttpPut]
        public void Put([FromBody]Customer customer)
        {
            if (customer == null) throw new ArgumentNullException("customer");

            // update current stock data
            _customerRepository.Update(customer);            
        }

        [HttpDelete]
        public void Delete(Guid id)
        {
            var customer = _customerRepository.GetById(id);

            if (customer == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            _customerRepository.Remove(customer);
        }
    }
}
