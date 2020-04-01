using BaseWebApi.Models;
using BaseWebApi.repository;
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
    public class PayslipController : ApiController
    {
        private readonly IGenericRepository<Payslip> _payslipRepository;        


        public PayslipController(IGenericRepository<Payslip> payslipRepository)
        {
            _payslipRepository = payslipRepository;            

        }

        [HttpGet]
        public IQueryable<Payslip> Get()
        {
            return _payslipRepository.GetAll( c => c.Category, d => d.SubCategory).OrderBy(or => or.PayslipYear);

        }

        [HttpGet]
        public Payslip GetUnique(int id)
        {
            return _payslipRepository.GetAll().Where(j => j.PayslipId == id).FirstOrDefault();
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody]Payslip pay)
        {
            //if (!ModelState.IsValid) return Request.CreateResponse(HttpStatusCode.BadRequest);

            _payslipRepository.Add(pay);

            var savedEntity = _payslipRepository.Add(pay);
            var response = Request.CreateResponse(HttpStatusCode.Created, savedEntity);

            return response;
        }

        [HttpPut]
        public void Put([FromBody]Payslip pay)
        {
            if (pay == null) throw new ArgumentNullException("quiz");

            _payslipRepository.Update(pay);
        }

        [HttpDelete]
        public void Delete(int id)
        {
            var pay = _payslipRepository.GetAll().Where(x => x.PayslipId == id).FirstOrDefault();

            if (pay == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            _payslipRepository.Remove(pay);

            var fileName = pay.Frequency.ToString() + pay.CategoryId.ToString() + pay.SubCategoryId.ToString() + pay.PayslipMonth + pay.PayslipYear + ".pdf";
            var filePath = HttpContext.Current.Server.MapPath("~/ippisPayslip/" + fileName);
            if (File.Exists(filePath))
            {               
                File.Delete(filePath);
                //File.Move(fullPath, trashPath);
            }
        }

        public async Task<HttpResponseMessage> UploadPayslip(int id, int id2, string id3, string id4, int id5)
        {
            string chkname = id5.ToString() + id.ToString() + id2.ToString() + id4 + id3 + ".pdf";
            var checkPath = HttpContext.Current.Server.MapPath("~/ippisPayslip/" + chkname);
            if (File.Exists(checkPath))
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Conflict);
                return response;
                //File.Delete(Path);
                //File.Move(fullPath, trashPath);
            }
            else
            {
                string fileName = string.Empty;

                HttpResponseMessage response = new HttpResponseMessage();

                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count > 0)
                {
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];
                        //fileName = postedFile.FileName; id = category, id2 = subcategory, id3 = year, id4 = month
                        fileName = id5.ToString() + id.ToString() + id2.ToString() + id4 + id3 + ".pdf";
                        var filePath = HttpContext.Current.Server.MapPath("~/ippisPayslip/" + fileName);
                        postedFile.SaveAs(filePath);

                        Payslip slip = new Payslip();
                        slip.CategoryId = id;
                        slip.SubCategoryId = id2;
                        slip.PayslipMonth = id4;
                        slip.PayslipYear = id3;
                        slip.Frequency = id5;
                        _payslipRepository.Add(slip);
                        //string Path = System.Web.Hosting.HostingEnvironment.MapPath(@"~/holder2/");
                    }
                }
                return response;
            }  
        }

    }
}
