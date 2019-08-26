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
        private readonly IGenericRepository<Audit_StockIn> _auditStockinRepository;

        public StockinController(IGenericRepository<StockIn> stockinRepository,
                                IGenericRepository<Audit_StockIn> auditStockinRepository)
        {
            _stockinRepository = stockinRepository;
            _auditStockinRepository = auditStockinRepository;
        }

        [HttpGet]
        public IQueryable<StockIn> Get()
        {            
            return _stockinRepository.GetAll(c => c.Supplier, c => c.CompanyStockTag).Where(st => st.Status == true);
            //return _stockinRepository.Include(c => c.Supplier);    
            
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody]StockIn stockin)
        {
            if (!ModelState.IsValid) return Request.CreateResponse(HttpStatusCode.BadRequest);
            stockin.id = Guid.NewGuid();
            stockin.CreatedDate = DateTime.Now;
            stockin.Status = true;
            var savedEntity = _stockinRepository.Add(stockin);
            // Save stockin to audit tray
            Audit_StockIn auditData = auditHelper(stockin, "Created");
            var auditEntity = _auditStockinRepository.Add(auditData);
            var response = Request.CreateResponse(HttpStatusCode.Created, savedEntity);

            return response;
        }

        [HttpPut]
        public void Put([FromBody]StockIn stockin)
        {
            if (stockin == null) throw new ArgumentNullException("stockin");

            _stockinRepository.Update(stockin);
            // update stock in audit tray
            Audit_StockIn auditData = auditHelper(stockin, "Updated");
            var auditEntity = _auditStockinRepository.Add(auditData);
        }

        [HttpDelete]
        public void Delete(Guid id)
        {
            var stockin = _stockinRepository.GetById(id);

            if (stockin == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            _stockinRepository.Remove(stockin);
        }

        //method to create and save stock tag audit tray
        public Audit_StockIn auditHelper(StockIn stockin, string tag)
        {
            Audit_StockIn auditStockIn = new Audit_StockIn();
            auditStockIn.id = Guid.NewGuid();
            auditStockIn.StockInId = stockin.id;
            auditStockIn.SupplierId = stockin.SupplierId;
            auditStockIn.QuantitySupplied = stockin.QuantitySupplied;
            //auditStockIn.SuppliedPrice = stockin.SuppliedPrice;       ---- will adjust from database
            auditStockIn.UnitPrice = stockin.UnitPrice;
            auditStockIn.DateSupplied = stockin.DateSupplied;
            auditStockIn.PackUnit = stockin.PackUnit;
            auditStockIn.BatchNo = stockin.BatchNo;
            auditStockIn.CreatedUser = stockin.CreatedUser;
            auditStockIn.CreatedDate = stockin.CreatedDate;
            //auditStockIn.Status = stockin.Status;    ---- will adjust from database
            auditStockIn.ActivityStatus = tag;            

            return auditStockIn;

        }

    }
}
