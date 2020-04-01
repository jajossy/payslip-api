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
    public class CategoryController : ApiController
    {
        private readonly IGenericRepository<Category> _categoryRepository;        

        public CategoryController(IGenericRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;            

        }

       
        [HttpGet]
        public IQueryable<Category> Get()
        {           
            return _categoryRepository.GetAll();
        }

        [HttpGet]
        public IQueryable<Category> GetCatById(int id)
        {
            
            return _categoryRepository.GetAll().Where(x => x.CategoryId == id);

        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody]Category category)
        {
            if (!ModelState.IsValid) return Request.CreateResponse(HttpStatusCode.BadRequest);

            //category.CategoryId = Guid.NewGuid();
            var savedEntity = _categoryRepository.Add(category);
            var response = Request.CreateResponse(HttpStatusCode.Created, savedEntity);

            return response;            
        }

        [HttpPut]
        public void Put([FromBody]Category category)
        {            
            if (category == null) throw new ArgumentNullException("category");

            _categoryRepository.Update(category);
        }

        [HttpDelete]
        public void Delete(Guid id)
        {
            var category = _categoryRepository.GetById(id);

            if (category == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            _categoryRepository.Remove(category);
        }
    }
}
