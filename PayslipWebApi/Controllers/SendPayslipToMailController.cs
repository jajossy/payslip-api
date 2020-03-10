using BaseWebApi.Models;
using BaseWebApi.repository;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Web.Http;

namespace BaseWebApi.Controllers
{
    public class SendPayslipToMailController : ApiController
    {
        private readonly IGenericRepository<PayslipConfig> _configRepository;
        private readonly IGenericRepository<StaffDataLatest> _staffRepository;

        public SendPayslipToMailController(IGenericRepository<StaffDataLatest> staffRepository, 
                                            IGenericRepository<PayslipConfig> configRepository)
        {
            _staffRepository = staffRepository;
            _configRepository = configRepository;
        }

        [HttpGet]
        public HttpResponseMessage SendPayslip(string id, string id2, string id3, string id4, string id5)
        {
            // comments about id
            /*let id5 = slot;
            let id4 = platform;
            let id3 = year;
            let id2 = month;
            let id = Employee No.; */

             // payslip file path
             string payslipPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/ippisPayslip/");
            string payslipFileName = id2+id3+".pdf";
            string _templateName = string.Format("{0}{1}", payslipPath, payslipFileName);

            String source_file = _templateName;

            // temporary holder for created payslip
            string holderPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/holder/");
            // temporary holder for created payslip
            string holderPath2 = System.Web.Hosting.HostingEnvironment.MapPath(@"~/holder2/");

            String result = holderPath;
            String result2 = holderPath2;

            // locate the payslip page number in master file
            var configInfo = _configRepository.GetAll().Where(x => x.IppisNo == id && x.MonthTag == id2 && x.Year == id3
                                                                && x.Platform == id4 && x.Slot == id5).ToList();

            if (configInfo != null && configInfo.Count() <= 1)
            {
                PdfCopy copy;

                //create PdfReader object
                PdfReader reader = new PdfReader(source_file);

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    if (i == configInfo.FirstOrDefault().PdfPageNo)
                    {
                        //create Document object
                        Document document = new Document();
                        copy = new PdfCopy(document, new FileStream(result + configInfo.FirstOrDefault().IppisNo + ".pdf", FileMode.Create));
                        //open the document 
                        document.Open();
                        //add page to PdfCopy 
                        copy.AddPage(copy.GetImportedPage(reader, i));
                        //close the document object
                        document.Close();

                        // break the loop
                        break;
                        
                    }
                }

                reader.Close();

                /*HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                FileStream fileStream = File.OpenRead(holderPath + configInfo.IppisNo + ".pdf");
                response.Content = new StreamContent(fileStream);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

                return response;*/

                

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            else if (configInfo != null && configInfo.Count() > 1)
            {
                int monitor = 0;
                    

                    //create PdfReader object
                    PdfReader reader = new PdfReader(source_file);

                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        if (i == configInfo[0].PdfPageNo)
                        {
                        monitor++;
                        PdfCopy copy;
                        //create Document object
                        Document document = new Document();
                            copy = new PdfCopy(document, new FileStream(result + configInfo.FirstOrDefault().IppisNo + ".pdf", FileMode.Create));
                            //open the document 
                            document.Open();
                            //add page to PdfCopy 
                            copy.AddPage(copy.GetImportedPage(reader, i));
                            //close the document object
                            document.Close();

                            // break the loop
                            if(monitor == 2) break;

                        }
                        if (i == configInfo[1].PdfPageNo)
                        {
                        monitor++;
                        PdfCopy copy2;
                        //create Document object
                        Document document2 = new Document();
                            copy2 = new PdfCopy(document2, new FileStream(result2 + configInfo.FirstOrDefault().IppisNo + ".pdf", FileMode.Create));
                            //open the document 
                            document2.Open();
                            //add page to PdfCopy 
                            copy2.AddPage(copy2.GetImportedPage(reader, i));
                            //close the document object
                            document2.Close();

                            // break the loop
                            if (monitor == 2) break;

                    }

                    }

                    reader.Close();

                    /*HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    FileStream fileStream = File.OpenRead(holderPath + configInfo.IppisNo + ".pdf");
                    response.Content = new StreamContent(fileStream);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

                    return response;*/



                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
            else
            {
                // If something fails or somebody calls invalid URI, throw error.
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

        }

        

    }
}
