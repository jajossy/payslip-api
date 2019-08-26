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
    public class CurrentStockController : ApiController
    {
        private readonly IGenericRepository<CurrentStock> _currentStockRepository;
        private readonly IGenericRepository<Audit_CurrentStock> _auditCurrentStockRepository;

        public CurrentStockController(IGenericRepository<CurrentStock> currentStockRepository,
                                  IGenericRepository<Audit_CurrentStock> auditCurrentStockRepository)
        {
            _currentStockRepository = currentStockRepository;
            _auditCurrentStockRepository = auditCurrentStockRepository;
        }

        [HttpGet]
        public IQueryable<CurrentStock> Get()
        {
            return _currentStockRepository.GetAll(c => c.CompanyStockTag);
        }

        // check if stock already exist in company inventory
        [HttpGet]
        public CurrentStock GetById(Guid id)
        {
            return _currentStockRepository.GetAll(c => c.CompanyStockTag).Where(q => q.StockNameId == id).FirstOrDefault();
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody]CurrentStock currentStock)
        {
            if (!ModelState.IsValid) return Request.CreateResponse(HttpStatusCode.BadRequest);
            currentStock.id = Guid.NewGuid();
            currentStock.Status = false;
            // Save Current Stock data
            var savedEntity = _currentStockRepository.Add(currentStock);
            // Save Current Stock to audit tray
            Audit_CurrentStock auditData = auditHelper(currentStock, "Created");
            var auditEntity = _auditCurrentStockRepository.Add(auditData);
            var response = Request.CreateResponse(HttpStatusCode.Created, savedEntity);

            return response;
            //return null;
        }

        [HttpPut]
        public void Put([FromBody]CurrentStock currentStock)
        {
            if (currentStock == null) throw new ArgumentNullException("currentStock");

            // update current stock data
            _currentStockRepository.Update(currentStock);
            // update current stock audit tray
            Audit_CurrentStock auditData = auditHelper(currentStock, "Updated");
            var auditEntity = _auditCurrentStockRepository.Add(auditData);
        }

        [HttpDelete]
        public void Delete(Guid id)
        {
            var currentstock = _currentStockRepository.GetById(id);

            if (currentstock == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            _currentStockRepository.Remove(currentstock);
        }

        //method to create and current stock audit tray
        public Audit_CurrentStock auditHelper(CurrentStock currentStock, string tag)
        {
            Audit_CurrentStock auditCurrentStock = new Audit_CurrentStock();
            
            auditCurrentStock.id = Guid.NewGuid();
            auditCurrentStock.StocknameId = currentStock.StockNameId;
            auditCurrentStock.Quantity = currentStock.Quantity;
            auditCurrentStock.ReorderLevel = currentStock.ReorderLevel;
            auditCurrentStock.PackUnit = currentStock.PackUnit;
            //auditCurrentStock.SupplierUnitPrice = currentStock.SupplierUnitPrice;  --will resolve from database later
            auditCurrentStock.CompanyUnitPrice = currentStock.CompanyUnitPrice;
            auditCurrentStock.CreatedUser = currentStock.CreatedUser;
            auditCurrentStock.DateEntered = DateTime.Now;
            auditCurrentStock.Comment = currentStock.Comment;


            return auditCurrentStock;

        }
    }
}
