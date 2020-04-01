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
    public class SubCategoryController : ApiController
    {
        private readonly IGenericRepository<SubCategory> _subcategoryRepository;

        public SubCategoryController(IGenericRepository<SubCategory> subcategoryRepository)
        {
            _subcategoryRepository = subcategoryRepository;

        }


        [HttpGet]
        public IQueryable<SubCategory> Get()
        {
            return _subcategoryRepository.GetAll();
        }

        [HttpGet]
        public IQueryable<SubCategory> GetCatById(int id)
        {

            return _subcategoryRepository.GetAll().Where(x => x.CategoryId == id);

        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody]SubCategory subcategory)
        {
            //if (!ModelState.IsValid) return Request.CreateResponse(HttpStatusCode.BadRequest);

            //subcategory.SubCategoryId = Guid.NewGuid();
            var savedEntity = _subcategoryRepository.Add(subcategory);
            var response = Request.CreateResponse(HttpStatusCode.Created, savedEntity);

            return response;
        }

        [HttpPut]
        public void Put([FromBody]SubCategory subcategory)
        {
            if (subcategory == null) throw new ArgumentNullException("subcategory");

            _subcategoryRepository.Update(subcategory);
        }

        [HttpDelete]
        public void Delete(Guid id)
        {
            var subcategory = _subcategoryRepository.GetById(id);

            if (subcategory == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            _subcategoryRepository.Remove(subcategory);
        }
    }
}
