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
    public class SaleController : ApiController
    {
        private readonly IGenericRepository<Sale> _saleRepository;

        public SaleController(IGenericRepository<Sale> saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public IQueryable<Sale> Get()
        {
            return _saleRepository.GetAll();
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody]Sale sale)
        {
            if (!ModelState.IsValid) return Request.CreateResponse(HttpStatusCode.BadRequest);
            sale.id = Guid.NewGuid();
            sale.DateCreated = DateTime.Now;
            // Save sale data
            var savedEntity = _saleRepository.Add(sale);
            var response = Request.CreateResponse(HttpStatusCode.Created, savedEntity);

            return response;
        }

        [HttpPut]
        public void Put([FromBody]Sale sale)
        {
            if (sale == null) throw new ArgumentNullException("sale");

            // update sale data
            _saleRepository.Update(sale);
        }

        [HttpDelete]
        public void Delete(Guid id)
        {
            var sale = _saleRepository.GetById(id);

            if (sale == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            _saleRepository.Remove(sale);
        }
    }
}
