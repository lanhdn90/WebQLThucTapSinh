using Newtonsoft.Json;
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
    public class CompanyController : Controller
    {
        // GET: Company
        public ActionResult Index()
        {
            var list = new Share().listOrgan(2).OrderByDescending(x => x.StartDay).ToList();
            SetViewBagM();
            return View(list);
        }

        public void SetViewBagM(string selectedID = null)
        {
            SelectList Month = new SelectList(new[] {
                new {Text = "Một Tháng", Value = 1},
                new {Text = "Ba Tháng", Value = 3},
                new {Text = "Sáu Tháng", Value = 6},
                new {Text = "Một Năm", Value = 12},
            }, "Value", "Text");
            ViewBag.Month = Month;
        }

        [HttpPost]
        public JsonResult ChangeStatus(string id)
        {
            var res = new Share().ChangeStatus(id, 2);
            return Json(new
            {
                status = res
            });
        }

        public bool extension(string id, int val)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                var model = database.Person.SingleOrDefault(x => x.CompanyID == id && x.RoleID == 2);
                return (new Share().Extension(model.PersonID, val));
            }
            catch
            {
                return false;
            }
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
                WebDatabaseEntities database = new WebDatabaseEntities();
                Company organ = new Company();
                if (organ.Create(organization, 2))
                {
                    var modle = new Share().listOrgan(2).OrderByDescending(x => x.StartDay).ToList();
                    var comid = modle[0].ID;
                    var find = database.Person.SingleOrDefault(x => x.CompanyID == comid);
                    SendMailTK(find.PersonID);
                    ModelState.AddModelError("", "Thêm Công ty thành công");
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
                var res = database.Person.Find(personID);
                string cus = res.LastName + " " + res.FirstName;
                var com = database.Organization.Find(res.CompanyID);
                string compa = com.Name;
                string email = res.Email;
                string nd = personID;
                string content = System.IO.File.ReadAllText(Server.MapPath("~/Email/SendEmailPerson.html"));
                content = content.Replace("{{CustomerName}}", cus);
                content = content.Replace("{{CompanyName}}", compa);
                content = content.Replace("{{noidung}}", nd);

                var fromEmailAddress = ConfigurationManager.AppSettings["FromEmailAddress"].ToString();
                var fromEmailPassword = ConfigurationManager.AppSettings["FromEmailPassword"].ToString();
                var smtpHost = ConfigurationManager.AppSettings["SMTPHost"].ToString();

                MailMessage message = new MailMessage(fromEmailAddress, email);
                message.Subject = "Thông báo đăng ký Tài khoản";
                message.IsBodyHtml = true;
                message.Body = content;

                SmtpClient client = new SmtpClient(smtpHost, 587);
                client.EnableSsl = true;
                client.Timeout = 100000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                NetworkCredential nc = new NetworkCredential(fromEmailAddress, fromEmailPassword);
                //NetworkCredential nc = new NetworkCredential("htkhdreamteamdn@gmail.com", "Tinhban098");
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

        [HttpGet]
        public ActionResult Edit(string id)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            if (id == null)
            {
                id = Session["CompanyID"].ToString();
            }
            var model = database.Organization.SingleOrDefault(x => x.ID == id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Organization organi)
        {
            if (ModelState.IsValid)
            {

                var model = new Company().UpdateOrganization(organi, 2);
                if (model)
                {
                    ModelState.AddModelError("", "Cập nhật thành công");
                }
                else
                {
                    ModelState.AddModelError("", "Cập nhật thất bại");
                }
            }
            return View("Edit");
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            
            JsonSerializerSettings json = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var result = "";
            if (DeleteIntern(id) == true)
            {
                if (new Company().DeleteListLeader(id) == true)
                {
                    if (new Company().DeleteListInternShipNo(id) == true)
                    {
                        if(new Share().DeleteOrganzation(id, 2) == true)
                        {
                            result = JsonConvert.SerializeObject("Xóa công ty thành công", Formatting.Indented, json);
                        }
                        else
                        {
                            result = JsonConvert.SerializeObject("Xảy ra lỗi xóa công ty", Formatting.Indented, json);
                        }


                    }
                    else
                    {
                        result = JsonConvert.SerializeObject("Không thể xóa Khóa học", Formatting.Indented, json);
                    }
                }
                else
                {
                    result = JsonConvert.SerializeObject("Không thể xóa Hướng dẫn viên", Formatting.Indented, json);
                }
            }
            else
            {
                result = JsonConvert.SerializeObject("Không thể xóa thực tập sinh", Formatting.Indented, json);
            }
            return this.Json(result, JsonRequestBehavior.AllowGet);
        }

        //Xóa thực tập sinh dựa vào companyID
        public bool DeleteIntern(string id)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                var listIntern = database.Person.Where(x => x.CompanyID == id && x.RoleID == 5).ToList();
                if (listIntern != null)
                {
                    foreach (var item in listIntern)
                    {
                        new Share().DeletePerson(item.PersonID);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }

        }



    }
}