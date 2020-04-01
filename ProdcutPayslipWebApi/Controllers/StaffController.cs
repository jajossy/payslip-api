using BaseWebApi.Models;
using BaseWebApi.Providers;
using BaseWebApi.repository;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
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
            return _staffRepository.GetAll().Take(100);
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

        [HttpDelete]
        public void Delete(Guid id)
        {
            var staff = _staffRepository.GetById(id);

            if (staff == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            _staffRepository.Remove(staff);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> UploadStaffExcel()
        {
            string fileName = string.Empty;

            HttpResponseMessage response = new HttpResponseMessage();

            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                foreach (string file in httpRequest.Files)
                {

                    var postedFile = httpRequest.Files[file];

                    var studExcel = new List<staffExcel>();

                    using (var stream = new MemoryStream())
                    {
                        await postedFile.InputStream.CopyToAsync(stream);

                        using (var package = new ExcelPackage(stream))
                        {
                            var excel = new ExcelPackage(stream);
                            var workbook = excel.Workbook;
                            try
                            {
                                var sheet = excel.Workbook.Worksheets.First();
                                var rowCount = sheet.Dimension.Rows;

                                for (int row = 2; row <= rowCount; row++)
                                {
                                    studExcel.Add(new staffExcel
                                    {
                                        Ippis = sheet.Cells[row, 1].Value.ToString().Trim(),
                                        Surname = sheet.Cells[row, 2].Value.ToString().Trim(),
                                        Firstname = sheet.Cells[row, 3].Value.ToString().Trim(),
                                        Othername = sheet.Cells[row, 4].Value.ToString().Trim(),
                                        Fullname = sheet.Cells[row, 5].Value.ToString().Trim(),
                                        Department = sheet.Cells[row, 6].Value.ToString().Trim(),
                                        Email = sheet.Cells[row, 7].Value.ToString().Trim()
                                    });
                                }

                                // save up options
                                int counter = 0;

                                foreach (var item in studExcel)
                                {
                                    var checkIppis = _staffRepository.GetAll().Where(x => x.Ippis == item.Ippis).FirstOrDefault();
                                    if(checkIppis == null)
                                    {
                                        StaffDataLatest staff = new StaffDataLatest();
                                        staff.Ippis = item.Ippis;
                                        staff.Surname = item.Surname;
                                        staff.Firstname = item.Firstname;
                                        staff.Othername = item.Othername;
                                        staff.Fullname = item.Fullname;
                                        staff.Department = item.Department;
                                        staff.Email = item.Email;
                                        staff.Active = true;

                                        var savedEntity = _staffRepository.Add(staff);

                                    }
                                }
                            }
                            catch (Exception)//open xls file
                            {
                                //if its a .xls file it will throw an Exception              
                            }

                        }
                    }

                }
            }



            return response;
        }

        public class staffExcel
        {
            public string Ippis { get; set; }
            public string Surname { get; set; }
            public string Firstname { get; set; }
            public string Othername { get; set; }
            public string Fullname { get; set; }
            public string Department { get; set; }
            public string Email { get; set; }            
        }
    }
}
