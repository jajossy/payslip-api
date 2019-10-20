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
            var user =  _userRepository.GetAll();
            foreach(var u in user)
            {
                var convertPassword = EncryptDecrypt.Decrypt(u.password);
                u.password = convertPassword;
            }
            return user;
        }

        [HttpGet]
        public IQueryable<User> GetById(Guid id)
        {
            var user = _userRepository.GetAll().Where(x => x.id == id);
            foreach (var u in user)
            {
                var convertPassword = EncryptDecrypt.Decrypt(u.password);
                u.password = convertPassword;
            }
            return user;
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody]User user)
        {
            if (!ModelState.IsValid) return Request.CreateResponse(HttpStatusCode.BadRequest);
            user.id = Guid.NewGuid();
            user.password = EncryptDecrypt.Encrypt(user.password);           
            user.DateCreated = DateTime.Now;
            // Save Order Current Stock data
            var savedEntity = _userRepository.Add(user);            
            var response = Request.CreateResponse(HttpStatusCode.Created, savedEntity);

            return response;
        }

        [HttpPut]
        public void Put([FromBody]User user)
        {
            if (user == null) throw new ArgumentNullException("agent");
            user.password = EncryptDecrypt.Encrypt(user.password);
            // update current stock data
            _userRepository.Update(user);
        }
    }
}
