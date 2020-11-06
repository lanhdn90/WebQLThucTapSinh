using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WebQLThucTapSinh.Common;
using WebQLThucTapSinh.Models;

namespace WebQLThucTapSinh.Controllers
{
    public class CompanysController : Controller
    {
        // GET: Companys
        public ActionResult Index()
        {
            var listCompanys = new CompanyAndSchool().listOrgan(2).OrderByDescending(x => x.StartDay).ToList();
            SetViewBagMonth();
            return View(listCompanys);
        }

        public void SetViewBagMonth(string selectedID = null)
        {
            SelectList Month = new SelectList(new[] {
                new {Text = "Một Tháng", Value = 1},
                new {Text = "Ba Tháng", Value = 3},
                new {Text = "Sáu Tháng", Value = 6},
                new {Text = "Một Năm", Value = 12},
            }, "Value", "Text");
            ViewBag.Month = Month;
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Organization organization)
        {
            if (ModelState.IsValid)
            {
                CompanyAndSchool organ = new CompanyAndSchool();
                if (organ.Create(organization, 2))
                {
                    WebDatabaseEntities database = new WebDatabaseEntities();
                    var modle = new CompanyAndSchool().listOrgan(2).OrderByDescending(x => x.StartDay).ToList();
                    var comid = modle[0].ID;
                    var find = database.Person.SingleOrDefault(x => x.CompanyID == comid);
                    SendMailTK(find.PersonID);
                    return RedirectToAction("Index", modle);
                }
                else
                {
                    ModelState.AddModelError("", "Thêm Công ty thất bại");
                }
            }
            return View("Create");
        }



        public bool SendMailTK(string personID)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                var person = database.Person.Find(personID);
                string cus = person.LastName + " " + person.FirstName;
                var com = database.Organization.Find(person.CompanyID);
                string compa = com.Name;
                string email = person.Email;
                string nd = personID;
                string content = System.IO.File.ReadAllText(Server.MapPath("~/Email/EmailCompany.html"));
                content = content.Replace("{{CustomerName}}", cus);
                content = content.Replace("{{CompanyName}}", compa);
                content = content.Replace("{{noidung}}", nd);

                var fromEmailAddress = ConfigurationManager.AppSettings["FromEmailAddress"].ToString();
                var fromEmailPassword = ConfigurationManager.AppSettings["FromEmailPassword"].ToString();
                var smtpHost = ConfigurationManager.AppSettings["SMTPHost"].ToString();

                MailMessage message = new MailMessage(fromEmailAddress, email);
                message.Subject = "Thông báo Đăng ký Tài khoản";
                message.IsBodyHtml = true;
                message.Body = content;

                SmtpClient client = new SmtpClient(smtpHost, 587);
                client.EnableSsl = true;
                client.Timeout = 100000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                NetworkCredential nc = new NetworkCredential(fromEmailAddress, fromEmailPassword);
                client.UseDefaultCredentials = false;
                client.Credentials = nc;
                client.Send(message);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}