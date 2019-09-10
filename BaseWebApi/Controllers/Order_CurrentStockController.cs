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
    public class Order_CurrentStockController : ApiController
    {
        private readonly IGenericRepository<Order_CurrentStock> _orderCurrentStockRepository;
        private readonly IGenericRepository<Order_Audit_CurrentStock> _orderAuditCurrentStockRepository;

        public Order_CurrentStockController(IGenericRepository<Order_CurrentStock> orderCurrentStockRepository,
                                  IGenericRepository<Order_Audit_CurrentStock> orderAuditCurrentStockRepository)
        {
            _orderCurrentStockRepository = orderCurrentStockRepository;
            _orderAuditCurrentStockRepository = orderAuditCurrentStockRepository;
        }

        
        [HttpGet]
        public IQueryable<Order_CurrentStock> Get()
        {
            return _orderCurrentStockRepository.GetAll(c => c.CompanyStockTag);
        }

        /*[HttpGet]
        public IQueryable<Order_CurrentStock> GetOrder()
        {
            return _orderCurrentStockRepository.GetAll(c => c.CompanyStockTag);
        }*/

        [HttpPost]
        public HttpResponseMessage Post([FromBody]Order_CurrentStock orderCurrentStock)
        {
            if (!ModelState.IsValid) return Request.CreateResponse(HttpStatusCode.BadRequest);
            orderCurrentStock.id = Guid.NewGuid();
            // Save Order Current Stock data
            var savedEntity = _orderCurrentStockRepository.Add(orderCurrentStock);
            // Save Current Stock to audit tray
            Order_Audit_CurrentStock auditData = auditHelper(orderCurrentStock, "Created");
            var auditEntity = _orderAuditCurrentStockRepository.Add(auditData);
            var response = Request.CreateResponse(HttpStatusCode.Created, savedEntity);

            return response;
        }

        [HttpPut]
        public void Put([FromBody]Order_CurrentStock orderCurrentStock)
        {
            if (orderCurrentStock == null) throw new ArgumentNullException("orderCurrentStock");

            // update Order current stock data
            _orderCurrentStockRepository.Update(orderCurrentStock);
            // update Order current stock audit tray
            Order_Audit_CurrentStock auditData = auditHelper(orderCurrentStock, "Updated");
            var auditEntity = _orderAuditCurrentStockRepository.Add(auditData);
        }

        [HttpDelete]
        public void Delete(Guid id)
        {
            var orderCurrentStock = _orderCurrentStockRepository.GetById(id);

            if (orderCurrentStock == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            _orderCurrentStockRepository.Remove(orderCurrentStock);
        }

        //method to create and Order current stock audit tray
        public Order_Audit_CurrentStock auditHelper(Order_CurrentStock orderCurrentStock, string tag)
        {
            Order_Audit_CurrentStock orderAuditCurrentStock = new Order_Audit_CurrentStock();

            orderAuditCurrentStock.id = orderCurrentStock.id;
            orderAuditCurrentStock.StocknameId = orderCurrentStock.StockNameId;
            orderAuditCurrentStock.Quantity = orderCurrentStock.Quantity;
            orderAuditCurrentStock.ReorderLevel = orderCurrentStock.ReorderLevel;
            orderAuditCurrentStock.PackUnit = orderCurrentStock.PackUnit;
            //auditCurrentStock.SupplierUnitPrice = currentStock.SupplierUnitPrice;  --will resolve from database later
            orderAuditCurrentStock.CompanyUnitPrice = orderCurrentStock.CompanyUnitPrice;
            orderAuditCurrentStock.CreatedUser = orderCurrentStock.CreatedUser;
            orderAuditCurrentStock.DateEntered = DateTime.Now;
            orderAuditCurrentStock.Comment = orderCurrentStock.Comment;


            return orderAuditCurrentStock;

        }
    }
}
