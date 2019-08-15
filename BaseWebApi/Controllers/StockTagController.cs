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
    public class StockTagController : ApiController
    {
        private readonly IGenericRepository<CompanyStockTag> _tagRepository;
        private readonly IGenericRepository<Audit_CompanyStockTag> _auditTagRepository;

        public StockTagController(IGenericRepository<CompanyStockTag> tagRepository,
                                  IGenericRepository<Audit_CompanyStockTag> auditTagRepository)
        {
            _tagRepository = tagRepository;
            _auditTagRepository = auditTagRepository;
        }

        [HttpGet]
        public IQueryable<CompanyStockTag> Get()
        {
            return _tagRepository.GetAll();
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody]CompanyStockTag stocktag)
        {
            if (!ModelState.IsValid) return Request.CreateResponse(HttpStatusCode.BadRequest);
            stocktag.id = Guid.NewGuid();
            stocktag.Setdate = DateTime.Now;            
            var savedEntity = _tagRepository.Add(stocktag);
            Audit_CompanyStockTag auditData = auditHelper(stocktag, "Created");
            var auditEntity = _auditTagRepository.Add(auditData);
            var response = Request.CreateResponse(HttpStatusCode.Created, savedEntity);

            return response;
        }

        [HttpPut]
        public void Put([FromBody]CompanyStockTag stocktag)
        {
            if (stocktag == null) throw new ArgumentNullException("stocktag");

            _tagRepository.Update(stocktag);
        }

        [HttpDelete]
        public void Delete(Guid id)
        {
            var supplier = _tagRepository.GetById(id);

            if (supplier == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            _tagRepository.Remove(supplier);
        }

        public Audit_CompanyStockTag auditHelper(CompanyStockTag stockTag, string tag)
        {
            Audit_CompanyStockTag auditStockTaf = new Audit_CompanyStockTag();
            auditStockTaf.id = Guid.NewGuid();
            auditStockTaf.Stockname = stockTag.Stockname;
            auditStockTaf.CompanyPrice = stockTag.CompanyPrice;
            auditStockTaf.Setdate = stockTag.Setdate;
            auditStockTaf.Comment = stockTag.Comment;
            auditStockTaf.CreatedUser = stockTag.CreatedUser;
            auditStockTaf.CompanyStockTagId = stockTag.id;
            auditStockTaf.ActivityStatus = tag;

            return auditStockTaf;

        }
    }
}
