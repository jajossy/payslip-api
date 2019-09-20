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
        private readonly IGenericRepository<Order_CurrentStock> _orderCurrentStockRepository;
        private readonly IGenericRepository<Order_Audit_CurrentStock> _orderAuditCurrentStockRepository;


        public CurrentStockController(IGenericRepository<CurrentStock> currentStockRepository,
                                  IGenericRepository<Audit_CurrentStock> auditCurrentStockRepository,
                                  IGenericRepository<Order_CurrentStock> orderCurrentStockRepository,
                                  IGenericRepository<Order_Audit_CurrentStock> orderAuditCurrentStockRepository)
        {
            _currentStockRepository = currentStockRepository;
            _auditCurrentStockRepository = auditCurrentStockRepository;
            _orderCurrentStockRepository = orderCurrentStockRepository;
            _orderAuditCurrentStockRepository = orderAuditCurrentStockRepository;

        }

        [HttpGet]
        public IQueryable<CurrentStock> Get()
        {
            return _currentStockRepository.GetAll(c => c.CompanyStockTag);
        }

        [HttpGet]
        public IQueryable<Order_CurrentStock> GetOrder()
        {
            return _orderCurrentStockRepository.GetAll(c => c.CompanyStockTag);
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

            // Save also to order current stock table to monitor orderings
            Order_CurrentStock orderCurrentStock = new Order_CurrentStock();

            orderCurrentStock.id = currentStock.id;
            orderCurrentStock.StockNameId = currentStock.StockNameId;
            orderCurrentStock.Quantity = currentStock.Quantity;
            orderCurrentStock.ReorderLevel = currentStock.ReorderLevel;
            orderCurrentStock.PackUnit = currentStock.PackUnit;
            orderCurrentStock.CompanyUnitPrice = currentStock.CompanyUnitPrice;
            orderCurrentStock.SupplierUnitPrice = currentStock.SupplierUnitPrice;
            orderCurrentStock.Status = currentStock.Status;
            orderCurrentStock.Comment = currentStock.Comment;
            orderCurrentStock.CreatedUser = currentStock.CreatedUser;

            var savedOrderCurrent = _orderCurrentStockRepository.Add(orderCurrentStock);

            // Save Current Stock to audit tray
            Audit_CurrentStock auditData = auditHelper(currentStock, "Created");
            var auditEntity = _auditCurrentStockRepository.Add(auditData);

            // Save Order Current Stock to audit tray
            Order_Audit_CurrentStock auditData2 = auditHelper2(orderCurrentStock, "Created");
            var auditEntity2 = _orderAuditCurrentStockRepository.Add(auditData2);


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

            // update also to order current stock table to monitor orderings
            Order_CurrentStock orderCurrentStock = new Order_CurrentStock();

            orderCurrentStock.id = currentStock.id;
            orderCurrentStock.StockNameId = currentStock.StockNameId;
            orderCurrentStock.Quantity = currentStock.Quantity;
            orderCurrentStock.ReorderLevel = currentStock.ReorderLevel;
            orderCurrentStock.PackUnit = currentStock.PackUnit;
            orderCurrentStock.CompanyUnitPrice = currentStock.CompanyUnitPrice;
            orderCurrentStock.SupplierUnitPrice = currentStock.SupplierUnitPrice;
            orderCurrentStock.Status = currentStock.Status;
            orderCurrentStock.Comment = currentStock.Comment;
            orderCurrentStock.CreatedUser = currentStock.CreatedUser;

            // update current stock data
            _orderCurrentStockRepository.Update(orderCurrentStock);

            // update current stock audit tray
            Audit_CurrentStock auditData = auditHelper(currentStock, "Updated");
            var auditEntity = _auditCurrentStockRepository.Add(auditData);

            // update order current stock audit tray
            Order_Audit_CurrentStock auditData2 = auditHelper2(orderCurrentStock, "Updated");
            var auditEntity2 = _orderAuditCurrentStockRepository.Add(auditData2);
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
            auditCurrentStock.SupplierUnitPrice = currentStock.SupplierUnitPrice;
            auditCurrentStock.CompanyUnitPrice = currentStock.CompanyUnitPrice;
            auditCurrentStock.CreatedUser = currentStock.CreatedUser;
            auditCurrentStock.DateEntered = DateTime.Now;
            auditCurrentStock.Comment = currentStock.Comment;


            return auditCurrentStock;

        }

        //method to create and current stock audit tray
        public Order_Audit_CurrentStock auditHelper2(Order_CurrentStock orderCurrenttStock, string tag)
        {
            Order_Audit_CurrentStock auditOrderCurrentStock = new Order_Audit_CurrentStock();

            auditOrderCurrentStock.id = Guid.NewGuid();
            auditOrderCurrentStock.StocknameId = orderCurrenttStock.StockNameId;
            auditOrderCurrentStock.Quantity = orderCurrenttStock.Quantity;
            auditOrderCurrentStock.ReorderLevel = orderCurrenttStock.ReorderLevel;
            auditOrderCurrentStock.PackUnit = orderCurrenttStock.PackUnit;
            auditOrderCurrentStock.SupplierUnitPrice = orderCurrenttStock.SupplierUnitPrice;
            auditOrderCurrentStock.CompanyUnitPrice = orderCurrenttStock.CompanyUnitPrice;
            auditOrderCurrentStock.CreatedUser = orderCurrenttStock.CreatedUser;
            auditOrderCurrentStock.DateEntered = DateTime.Now;
            auditOrderCurrentStock.Comment = orderCurrenttStock.Comment;


            return auditOrderCurrentStock;

        }
    }
}
