using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Reflection;
//using LocalPurchaseApi.Models;
//using LocalPurchaseApi.Repositories;
//using LocalPurchaseApi.ViewModels;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using Spire.Xls;
using Spire.Xls.Converter;
using ExcelPicture = OfficeOpenXml.Drawing.ExcelPicture;
using FileFormat = Spire.Xls.FileFormat;
using BaseWebApi.Models;
using BaseWebApi.repository;

namespace LocalPurchaseApi.StreamData
{
    public static class GeneralMemoryStream
    {
        
        /*private static Order GetOrderData(Guid lpoId)
        {
            var _orderRepository = new GenericRepository<Order>();
            var _orderItemRepository = new GenericRepository<OrderItem>();
            var _customerRepository = new GenericRepository<Customer>();
            

            var orderData = _orderRepository.GetAll(c => c.OrderItems, c => c.Customer).Where(or => or.id == lpoId).FirstOrDefault();
            foreach(var pd in orderData.OrderItems)
            {
                var productData = _orderItemRepository.GetAll(p => p.CompanyStockTag).Where(p => p.id == pd.id).FirstOrDefault();
                pd.BatchNo = productData.CompanyStockTag.Stockname;
            }
            
            return orderData;
        }*/


        public static MemoryStream GetLPOList(Guid lpoId)
        {
            //string _templatePath = ConfigurationManager.AppSettings["TemplatePath"];
            string _templatePath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/ifluent-db/");
            //string _templateFileName = ConfigurationManager.AppSettings["LPOListFileName"];
            string _templateFileName = "OrderList.xlsx";
            string _templateName = string.Format("{0}{1}", _templatePath, _templateFileName);

            FileInfo _xlTemplate = new FileInfo(_templateName);
            // Results Output
            MemoryStream output = new MemoryStream();

            using (FileStream templateDocumentStream = File.OpenRead(_templateName))
            {
                using (ExcelPackage _xlPackage = new ExcelPackage(templateDocumentStream))
                {
                    ExcelWorksheet _ws = _xlPackage.Workbook.Worksheets["Full"];
                    _ws.PrinterSettings.FitToPage = false;


                    int startHeadingRow = 1;
                    int startSubHeadingRow = 4;
                    int startTableRow = 15;
                    int startUnderLineRow = 55;

                    int counter = 0;
                    //var provisionalList = GetOrderData(lpoId);
                    /*_ws.Names["ApprovalRefNo"].Value = provisionalList.OrderTag;
                    _ws.Names["DateOfApproval"].Value = provisionalList.DateCreated;
                    _ws.Names["DateEntered"].Value = provisionalList.DateCreated;
                    _ws.Names["TypeOfProduct"].Value = provisionalList.Approved;
                    _ws.Names["To"].Value = provisionalList.Customer.Storename;
                    _ws.Names["Destination"].Value = provisionalList.AgentId;
                    _ws.Names["RevolvingAccount"].Value = provisionalList.TotalOrderAmount;
                    _ws.Names["Others"].Value = provisionalList.TotalSuppliedAmount;
                    _ws.Names["LPONo"].Value = provisionalList.Customer.PhoneNo;
                    _ws.Names["Destination"].Value = provisionalList.TotalOrderAmount;
                    _ws.Names["TypeOfProduct"].Value = provisionalList.Customer.MarketPlace;
                    */
                    //_ws.Cells["B" + startSubHeadingRow].Value = j.LPONo;
                    //_ws.Cells["B" + (startSubHeadingRow + 1)].Value = j.Semester;
                    //_ws.Cells["B" + (startSubHeadingRow + 2)].Value = j.StudentLevel;
                    //_ws.Cells["I" + (startSubHeadingRow)].Value = j.ApprovalRefNo;
                    //_ws.Cells["I" + (startSubHeadingRow + 1)].Value = j.DateOfApproval;
                    //_ws.Cells["I" + (startSubHeadingRow + 2)].Value = j.SupplierId;
                    /*if (provisionalList.OrderItems != null && provisionalList.OrderItems.Count > 0)
                    {
                        foreach (var j in provisionalList.OrderItems)
                        {
                            _ws.Cells["A" + (startTableRow)].Value = j.BatchNo;
                            _ws.Cells["C" + (startTableRow)].Value = j.Quantity;
                            //_ws.Cells["D" + (startTableRow)].Value = j.SalesTotalAmount;
                            _ws.Cells["H" + (startTableRow)].Value = j.SalesUnitPrice.ToString("N");
                            _ws.Cells["I" + (startTableRow)].Value = j.SalesTotalAmount.ToString("N");
                            startTableRow++;
                        }
                    }*/
                   


                    _xlPackage.SaveAs(output);
                }
                return output;
            }
        }

        private static string GetCharacter(int number)
        {
            string contact = "";
            if (number >= 26)
            {
                number = number - 26;
                contact = "A";
                if (number >= 26)
                {
                    number = number - 26;
                }
                if (number >= 26)
                {
                    number = number - 26;
                }
                string i = Convert.ToChar((number + 65)).ToString();
                return contact + i.Trim();
            }
            else
            {
                string i = Convert.ToChar((number + 65)).ToString();
                return i;
            }
        }
     

        public static HttpResponseMessage ReturnStreamAsFile(MemoryStream stream, string filename, string format)
        {
            // Set HTTP Status Code
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);

            // Reset Stream Position
            stream.Position = 0;
            result.Content = new StreamContent(stream);

            // Generic Content Header

            switch (format)
            {
                case "PDF":
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                    result.Content.Headers.ContentDisposition =
                        new ContentDispositionHeaderValue(DispositionTypeNames.Inline);
                    break;

                case "XLSX":
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    result.Content.Headers.ContentDisposition =
                        new ContentDispositionHeaderValue(DispositionTypeNames.Inline);
                    break;
            }


            //Set Filename sent to client
            result.Content.Headers.ContentDisposition.FileName = filename;

            return result;
        }

        public static MemoryStream ConvertXLSXtoPDF(MemoryStream stream)
        {
            // Result Stream
            MemoryStream pdfMemoryStream = new MemoryStream();

            // Spire.XLS to open XLSX workbook stream created by EPPlus
            Workbook workbook = new Workbook();
            workbook.LoadFromStream(stream, ExcelVersion.Version2013);

            // Spire.PDF to convert XLSX to PDF, I read it has limited functionality (total pages, rows, etc...).
            PdfConverter pdfConverter = new PdfConverter(workbook);

            PdfConverterSettings settings = new PdfConverterSettings();

            settings.FitSheetToOnePage = FitToPageType.ScaleWithSameFactor;

            workbook.SaveToStream(pdfMemoryStream, FileFormat.PDF);
            //PdfDocument pdfDocument = pdfConverter.Convert(settings);          
            //pdfDocument.SaveToStream(pdfMemoryStream);

            return pdfMemoryStream;
        }

        public class LPOModel
        {
            public System.Guid id { get; set; }
            public System.Guid AgentId { get; set; }
            public System.Guid CustomerId { get; set; }
            public Nullable<bool> Approved { get; set; }
            public Nullable<bool> Pending { get; set; }
            public Nullable<bool> Cancelled { get; set; }
            public decimal TotalOrderAmount { get; set; }
            public Nullable<decimal> TotalSuppliedAmount { get; set; }
            public System.DateTime DateCreated { get; set; }
            public string PaymentType { get; set; }
            public string OrderTag { get; set; }
        }
    }
}