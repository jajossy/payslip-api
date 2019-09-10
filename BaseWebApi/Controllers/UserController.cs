using BaseWebApi.Models;
using BaseWebApi.Providers;
using BaseWebApi.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BaseWebApi.Controllers
{
    public class UserController : ApiController
    {
        private readonly IGenericRepository<User> _userRepository;        

        public UserController(IGenericRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public IQueryable<User> Get()
        {
            return _userRepository.GetAll();
        }

        [HttpGet]
        public User GetById(Guid id)
        {
            return _userRepository.GetAll().Where(x => x.id == id).FirstOrDefault();
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody]User user)
        {
            if (!ModelState.IsValid) return Request.CreateResponse(HttpStatusCode.BadRequest);
            user.id = Guid.NewGuid();
            user.password = EncryptDecrypt.Encrypt(user.password);            
            user.UserId = Guid.Parse("F65A4320-6ABE-4FDF-A395-0067C1FB2611");
            user.DateCreated = DateTime.Now;
            // Save Order Current Stock data
            var savedEntity = _userRepository.Add(user);            
            var response = Request.CreateResponse(HttpStatusCode.Created, savedEntity);

            return response;
        }
    }
}
