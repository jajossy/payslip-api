using LocalPurchaseApi.StreamData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BaseWebApi.Controllers
{
    public class GeneratePdfController : ApiController
    {
        string format = "PDF";
        public HttpResponseMessage GetReport(Guid id)
        {
            MemoryStream documentStream = GeneralMemoryStream.GetLPOList(id);

            // Make Sure Document is Loaded
            if (documentStream != null && documentStream.Length > 0)
            {
                string documentName = string.Format("ifluent-{0}.{1}", id, format);

                // Based on format requested in URI (PDF/XLSX), call the associated method.
                switch (format)
                {
                    case "PDF":
                        MemoryStream pdfMemoryStream = GeneralMemoryStream.ConvertXLSXtoPDF(documentStream);
                        return GeneralMemoryStream.ReturnStreamAsFile(pdfMemoryStream, documentName, format);
                        break;

                    case "XLSX":
                        return GeneralMemoryStream.ReturnStreamAsFile(documentStream, documentName, format);
                        break;
                }
            }

            // If something fails or somebody calls invalid URI, throw error.
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }
    }
}
