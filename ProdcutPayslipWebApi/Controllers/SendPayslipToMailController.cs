using BaseWebApi.Models;
using BaseWebApi.repository;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Web.Http;

namespace BaseWebApi.Controllers
{
    public class SendPayslipToMailController : ApiController
    {
        private readonly IGenericRepository<Payslip> _payslipRepository;
        private readonly IGenericRepository<StaffDataLatest> _staffRepository;

        public SendPayslipToMailController(IGenericRepository<StaffDataLatest> staffRepository, IGenericRepository<Payslip> payslipRepository)
        {
            _staffRepository = staffRepository;
            _payslipRepository = payslipRepository;
        }

        [HttpGet]
        public HttpResponseMessage SendPayslip(string id, string id2, string id3, string id4, string id5)
        {

            var payslipData = _payslipRepository.GetAll().Where(p => p.PayslipMonth == id4
            && p.PayslipYear == id3 && p.SubCategoryId.ToString() == id2 && p.CategoryId.ToString() == id).FirstOrDefault();
            // comments about id
            /*            
            let id = CategoryId;
              let id2 = SubCategoryId;
              let id3 = payslipyear;
              let id4 = payslipmonth;
              let id5 = Ippis;             
             */

            // payslip file path
            string payslipPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/ippisPayslip/");
            //string payslipFileName = id2+id3+".pdf";
            string payslipFileName = payslipData.Frequency.ToString() + id.ToString() + id2.ToString() + id4 + id3 + ".pdf";
            string _templateName = string.Format("{0}{1}", payslipPath, payslipFileName);

            string source_file = _templateName;

            // temporary holder for created payslip
            string holderPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/holder/");
            // temporary holder for created payslip
            string holderPath2 = System.Web.Hosting.HostingEnvironment.MapPath(@"~/holder2/");

            String result = holderPath;
            String result2 = holderPath2;

            List<ConfigRepo> pageInstance = new List<ConfigRepo>(); // where to hold config info

            // locate the payslip page number in master file
            /*var configInfo = _configRepository.GetAll().Where(x => x.IppisNo == id && x.MonthTag == id2 && x.Year == id3
                                                                && x.Platform == id4 && x.Slot == id5).ToList();*/


            using (PdfReader reader = new PdfReader(source_file))
            {
                //StringBuilder text = new StringBuilder();
                
                int count = 1;
                //ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {

                    StringBuilder text = new StringBuilder();
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    text.Clear();
                    text.Append(Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(PdfTextExtractor.GetTextFromPage(reader, i, strategy)))));
                    //text.Append(PdfTextExtractor.GetTextFromPage(reader, i));                   

                    // for Ippis Product Alone
                    


                    if (!string.IsNullOrWhiteSpace(text.ToString()))
                    {
                        string sb_final = text.ToString();
                        int new_file_name_index = sb_final.IndexOf("Number");
                        int startValue = new_file_name_index + 8;
                        string new_file_name = sb_final.Substring(startValue, 6);

                        if(new_file_name != " Oracl")
                        {
                            
                            //Console.WriteLine(count);
                            //Console.WriteLine(new_file_name);
                            //count++;
                            if(new_file_name == id5 && payslipData.Frequency == 1)
                            {
                                ConfigRepo pageFile = new ConfigRepo();
                                pageFile.IppisNo = id5;
                                pageFile.PageNo = i;
                                // save the page no.
                                pageInstance.Add(pageFile);
                                break;
                            }
                            else if (new_file_name == id5 && payslipData.Frequency == 2)
                            {
                                ConfigRepo pageFile = new ConfigRepo();
                                pageFile.IppisNo = id5;
                                pageFile.PageNo = i;
                                // save the page no.
                                pageInstance.Add(pageFile);
                            }
                        }                        
                        
                    }
                    


                }

            }

            var kk = pageInstance;

            if (pageInstance != null && pageInstance.Count() == 1)
            {
                // check to delete any file due to error first
                string fullPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/holder/" + pageInstance.FirstOrDefault().IppisNo + ".pdf");
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);                    
                }

                PdfCopy copy;

                //create PdfReader object
                PdfReader reader = new PdfReader(source_file);

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    if (i == pageInstance.FirstOrDefault().PageNo)
                    {
                        //create Document object
                        Document document = new Document();
                        copy = new PdfCopy(document, new FileStream(result + pageInstance.FirstOrDefault().IppisNo + ".pdf", FileMode.Create));
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
        else if (pageInstance != null && pageInstance.Count() > 1)
        {
            int monitor = 0;


                //create PdfReader object
                PdfReader reader = new PdfReader(source_file);

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    if (i == pageInstance[0].PageNo)
                    {
                        // check to delete any file that exist due to error first
                        string fullPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/holder/" + pageInstance[0].PageNo + ".pdf");
                        if (File.Exists(fullPath))
                        {
                            File.Delete(fullPath);
                        }

                        monitor++;
                        PdfCopy copy;
                        //create Document object
                        Document document = new Document();
                        copy = new PdfCopy(document, new FileStream(result + pageInstance.FirstOrDefault().IppisNo + ".pdf", FileMode.Create));
                        //open the document 
                        document.Open();
                        //add page to PdfCopy 
                        copy.AddPage(copy.GetImportedPage(reader, i));
                        //close the document object
                        document.Close();

                        // break the loop
                        if(monitor == 2) break;

                    }
                    if (i == pageInstance[1].PageNo)
                    {
                        // check to delete any file that exist due to error first
                        string fullPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/holder2/" + pageInstance[1].PageNo + ".pdf");
                        if (File.Exists(fullPath))
                        {
                            File.Delete(fullPath);
                        }

                        monitor++;
                        PdfCopy copy2;
                        //create Document object
                        Document document2 = new Document();
                        copy2 = new PdfCopy(document2, new FileStream(result2 + pageInstance.FirstOrDefault().IppisNo + ".pdf", FileMode.Create));
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
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        

    }

    public class ConfigRepo
    {
        public string IppisNo { get; set; }
        public int PageNo { get; set; }
    }
}
