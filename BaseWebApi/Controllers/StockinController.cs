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
    public class StockinController : ApiController
    {
        private readonly IGenericRepository<StockIn> _stockinRepository;        

        public StockinController(IGenericRepository<StockIn> stockinRepository)
        {
            _stockinRepository = stockinRepository;            
        }

        [HttpGet]
        public IQueryable<StockIn> Get()
        {            
            return _stockinRepository.GetAll(c => c.Supplier, c => c.CompanyStockTag);
            //return _stockinRepository.Include(c => c.Supplier);            
            
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody]StockIn stockin)
        {
            if (!ModelState.IsValid) return Request.CreateResponse(HttpStatusCode.BadRequest);
            stockin.id = Guid.NewGuid();
            stockin.CreatedDate = DateTime.Now;
            var savedEntity = _stockinRepository.Add(stockin);            
            var response = Request.CreateResponse(HttpStatusCode.Created, savedEntity);

            return response;
        }

        [HttpPut]
        public void Put([FromBody]StockIn stockin)
        {
            if (stockin == null) throw new ArgumentNullException("stockin");

            _stockinRepository.Update(stockin);
        }

        [HttpDelete]
        public void Delete(Guid id)
        {
            var supplier = _stockinRepository.GetById(id);

            if (supplier == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            _stockinRepository.Remove(supplier);
        }
        
    }
}
