using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace BaseWebApi.Controllers
{
    public class GetPdfController : ApiController
    {
        public HttpResponseMessage Get(string id, string id2)
        {
            // temporary holder for created payslip
            string holderPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/holder/");

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            FileStream fileStream = File.OpenRead(holderPath + id + ".pdf");
            response.Content = new StreamContent(fileStream);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

            /*string fullPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/holder/" + id + ".pdf");
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }*/

            return response;
        }
    }
}
