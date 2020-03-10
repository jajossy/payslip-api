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
    public class StaffController : ApiController
    {
        //private readonly IGenericRepository<StaffDataLatest> _userRepository;
        private readonly IGenericRepository<StaffDataLatest> _staffRepository;

        public StaffController( IGenericRepository<StaffDataLatest> staffRepository)
        {
            //_userRepository = userRepository;
            _staffRepository = staffRepository;

        }

        [HttpGet]
        public IQueryable<StaffDataLatest> GetByIppis(string id)
        {
            var staff = _staffRepository.GetAll().Where(x => x.Ippis == id);
            return staff;
        }

        [HttpGet]
        public IQueryable<StaffDataLatest> GetByName(string id)
        {

            return _staffRepository.GetAll().Where(x => x.Fullname.Contains(id));

        }

        [HttpGet]
        public IQueryable<StaffDataLatest> Get()
        {
            /*var user =  _userRepository.GetAll();
            foreach(var u in user)
            {
                var convertPassword = EncryptDecrypt.Decrypt(u.Password);
                u.Password = convertPassword;
            }
            return user;*/
            return _staffRepository.GetAll();
        }

        [HttpGet]
        public IQueryable<StaffDataLatest> GetById(int id)
        {
            /*var user = _userRepository.GetAll().Where(x => x.UserId == id);
            foreach (var u in user)
            {
                var convertPassword = EncryptDecrypt.Decrypt(u.Password);
                u.Password = convertPassword;
            }
            return user;*/
            return _staffRepository.GetAll().Where(x => x.id == id);
            
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody]StaffDataLatest staff)
        {
            if (!ModelState.IsValid) return Request.CreateResponse(HttpStatusCode.BadRequest);
           
            var savedEntity = _staffRepository.Add(staff);
            var response = Request.CreateResponse(HttpStatusCode.Created, savedEntity);

            return response;
            /*if (!ModelState.IsValid) return Request.CreateResponse(HttpStatusCode.BadRequest);
            user.UserId = Guid.NewGuid();
            user.Password = EncryptDecrypt.Encrypt(user.Password);           
            //user.DateCreated = DateTime.Now;
            // Save Order Current Stock data
            var savedEntity = _userRepository.Add(user);            
            var response = Request.CreateResponse(HttpStatusCode.Created, savedEntity);

            return response;*/
        }

        [HttpPut]
        public void Put([FromBody]StaffDataLatest staff)
        {
            /*if (user == null) throw new ArgumentNullException("agent");
            user.Password = EncryptDecrypt.Encrypt(user.Password);
            // update current stock data
            _userRepository.Update(user);*/

            if (staff == null) throw new ArgumentNullException("agent");
            
            _staffRepository.Update(staff);
        }
    }
}
