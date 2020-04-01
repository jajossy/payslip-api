using BaseWebApi.Models;
using BaseWebApi.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Http;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.IO;

namespace BaseWebApi.Controllers
{
    public class SendEmailController : ApiController
    {
        private readonly IGenericRepository<StaffDataLatest> _staffRepository;
        //private readonly IGenericRepository<PayslipConfig> _configRepository;
        public string recipientMail;
        public string recipientIppis;
        public string GlobalResponse;

        public SendEmailController(IGenericRepository<StaffDataLatest> staffRepository)
        {
            _staffRepository = staffRepository;
            //_configRepository = configRepository;
        }

        [HttpGet]
        public async Task<HttpResponseMessage> SendPayslip(string id, string id2, string id3)
        {
            var response = new HttpResponseMessage();
            // locate the payslip page number in master file
            var staffInfo = _staffRepository.GetAll().Where(x => x.Ippis == id).FirstOrDefault();

            if (!string.IsNullOrEmpty(staffInfo.Email))
            {
                bool testEmail = IsValidEmail(staffInfo.Email);
                if (testEmail)
                {
                    recipientMail = staffInfo.Email.ToLower();
                    recipientIppis = staffInfo.Ippis;
                    SmtpClient smtp = new SmtpClient("mail.uch-ibadan.org.ng", 22);
                    //client.Port = 587;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    NetworkCredential credentials = new NetworkCredential("payslip@uch-ibadan.org.ng", "payslipPWD123!");
                    smtp.EnableSsl = true;
                    smtp.Credentials = credentials;

                    var message = new MailMessage();
                    message.To.Add(new MailAddress(staffInfo.Fullname + " <" + staffInfo.Email.ToLower() + ">"));
                    //message.To.Add(new MailAddress("Jolaosho Joseph" + " <" + "jajossy2@gmail.com" + ">"));
                    message.From = new MailAddress("UCH, Ibadan - Payslip <payslip@uch-ibadan.org.ng>");
                    //message.From = new MailAddress("Payslip <payslip@uch-ibadan.org.ng>");
                    //message.Bcc.Add(new MailAddress("Amit Mohanty <amitmohanty@email.com>"));
                    message.Subject = "UCH: Payslip " + id2 + " " + id3 + " for IPPIS / GIFMIS Number " + staffInfo.Ippis;
                    //message.Subject = "Hospital: Payslip " + id2 + " " + id3 + " for IPPIS Number " + staffInfo.Ippis;
                   message.Body = staffInfo.Fullname + "<br />" +
                        " Find attached document for " + id2 + " "+ id3 + " payslip ....Finance and Account - UCH, Ibadan."
                        + "<br />" + "<br />" + "Powered By Information Technology Department";
                    /*message.Body = staffInfo.Fullname + "<br />" +
                        " Find attached document for " + id2 + " " + id3 + " payslip ....Finance and Account - Hospital."
                        + "<br />" + "<br />" + "Powered By Information Technology Department";*/
                    message.IsBodyHtml = true;

                    // initiate varaible for holder and holder2
                    string holderPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/holder/");
                    string holderPath2 = System.Web.Hosting.HostingEnvironment.MapPath(@"~/holder2/");

                    string file = holderPath + staffInfo.Ippis + ".pdf";
                    string file2 = holderPath2 + staffInfo.Ippis + ".pdf";

                    
                    if (File.Exists(file))
                    {
                        Attachment attachment;
                        attachment = new Attachment(file);
                        message.Attachments.Add(attachment);
                    }

                    if (File.Exists(file2))
                    {
                        Attachment attachment;
                        attachment = new Attachment(file2);
                        message.Attachments.Add(attachment);
                    }


                    //using (var smtp = new SmtpClient())
                    //{
                    //await smtp.SendMailAsync(message);
                    //await Task.FromResult(0);
                    //}
                    try
                    {
                        //smtp.SendCompleted += new SendCompletedEventHandler(SendCompletedCallBack);
                        smtp.SendCompleted += Smtp_SendCompleted;
                        //message.Dispose();
                        await smtp.SendMailAsync(message);
                        message.Dispose();

                        // delete the file after dispose
                       string fullPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/holder/" + staffInfo.Ippis + ".pdf");
                       

                       if (File.Exists(fullPath))
                       {
                           File.Delete(fullPath);
                           //File.Move(fullPath, trashPath);
                       }

                       
                        string fullPath2 = System.Web.Hosting.HostingEnvironment.MapPath(@"~/holder2/" + staffInfo.Ippis + ".pdf");
                        if (File.Exists(fullPath2))
                        {
                            File.Delete(fullPath2);
                            //File.Move(fullPath, trashPath);
                        }
                        
                        

                        await Task.FromResult(response);                      

                    }
                    catch (Exception ex)
                    {
                        message.Dispose();
                        response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                        await Task.FromResult(response);
                    }

                    message.Dispose();
                }
                
            }



            //return await new HttpResponseMessage(HttpStatusCode.OK)
            //return ResponseMessage(Request.CreateResponse(Request.CreateResponse(HttpStatusCode.OK)));
            return await Task<HttpResponseMessage>.Factory.StartNew(() =>
            {
               
                return Request.CreateResponse(HttpStatusCode.OK, GlobalResponse);
            });

        }

        public void Smtp_SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            //String token = (string)e.UserState; 

            /*string fullPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/holder/" + "201749" + ".pdf");
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }*/
            

            if (e.Cancelled)
            {
                Console.WriteLine("[{0}] Send canceled.", e.UserState.ToString());
                GlobalResponse = "Send canceled";
            }
            if (e.Error != null)
            {
                Console.WriteLine("[{0}] {1}", e.UserState.ToString(), e.Error.ToString());
                GlobalResponse = "Sent Error";
            }
            else
            {
                Console.WriteLine("Message sent.");
                var result = _staffRepository.GetAll().Where(b => b.Ippis == recipientIppis).FirstOrDefault();
                if (result != null)
                {
                    //result.Comment = "Message Sent";
                    _staffRepository.Update(result);
                    GlobalResponse = "Message Sent";
                }

                
                
                /*string fullPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/holder/" + "197514" + ".pdf");
                string trashPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/trash/" + "197514" + ".pdf");
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    //File.Move(fullPath, trashPath);
                }*/

                //foreach (var path in paths)
                    //System.IO.File.Delete(fullPath);
            }
            //mailSent = true;
        }

        

        public static bool IsValidEmail(string strIn)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }
    }
}

