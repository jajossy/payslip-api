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
    public class SupplierController : ApiController
    {
        private readonly IGenericRepository<Supplier> _supplierRepository;
        private readonly IGenericRepository<Audit_Supplier> _auditSupplierRepository;

        public SupplierController(IGenericRepository<Supplier> supplierRepository, 
                                  IGenericRepository<Audit_Supplier> auditSupplierRepository)
        {
            _supplierRepository = supplierRepository;
            _auditSupplierRepository = auditSupplierRepository;
        }

        [HttpGet]
        public IQueryable<Supplier> Get()
        {
            return _supplierRepository.GetAll();
        }

        [HttpPost]  
        public HttpResponseMessage Post([FromBody]Supplier supplier)  
        {            
            if (!ModelState.IsValid)  return Request.CreateResponse(HttpStatusCode.BadRequest);
            supplier.id = Guid.NewGuid();
            supplier.DateCreated = DateTime.Now;
            supplier.Status = true;
            var savedEntity = _supplierRepository.Add(supplier);
            Audit_Supplier auditData = auditHelper(supplier, "Created");
            var auditEntity = _auditSupplierRepository.Add(auditData);
            var response = Request.CreateResponse(HttpStatusCode.Created, savedEntity);
                        
            return response;
        }

        [HttpPut]
        public void Put([FromBody]Supplier supplier)
        {
            if (supplier == null) throw new ArgumentNullException("supplier");

            _supplierRepository.Update(supplier);
        }

        [HttpDelete]
        public void Delete(Guid id)
        {
            var supplier = _supplierRepository.GetById(id);

            if (supplier == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            _supplierRepository.Remove(supplier);
        }

        public Audit_Supplier auditHelper(Supplier supplier, string tag)
        {
            Audit_Supplier auditSupplier = new Audit_Supplier();
            auditSupplier.id = Guid.NewGuid();
            auditSupplier.CompanyName = supplier.CompanyName;
            auditSupplier.ContactName = supplier.ContactName;
            auditSupplier.ContactTitle = supplier.ContactTitle;
            auditSupplier.Address = supplier.Address;
            auditSupplier.CountryId = supplier.CountryId;
            auditSupplier.StateId = supplier.StateId;
            auditSupplier.Region = supplier.Region;
            auditSupplier.Phone = supplier.Phone;
            auditSupplier.DateCreated = supplier.DateCreated;
            auditSupplier.CreatedUser = supplier.CreatedUser;
            auditSupplier.Status = supplier.Status;
            auditSupplier.SupplierId = supplier.id;
            auditSupplier.ActivityStatus = tag;

            return auditSupplier;

        }


    }
}
