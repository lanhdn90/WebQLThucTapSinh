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
    public class RepresentativeController : Controller
    {
        // GET: Representative
        public ActionResult Index(string id)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            //var company = Session["CompanyID"].ToString();
            var company = "ASDFGHJR";
            var list = (from a in database.Person
                        join b in database.Users on a.PersonID equals b.PersonID
                        join c in database.Organization on a.CompanyID equals c.ID
                        where a.CompanyID == company && a.RoleID == 3
                        select new RepresentativeClass()
                        {
                            PersonID = a.PersonID,
                            FullName = a.LastName + " " + a.FirstName,
                            SchoolID = a.SchoolID,
                            Address = a.Address,
                            Phone = a.Phone,
                            Email = a.Email,
                            Status = b.Status,
                        }).ToList();
            if (id != null)
            {
                list = list.Where(x => x.SchoolID == id).ToList();
            }
            return View(list);
        }

        [HttpPost]
        public JsonResult ChangeStatus(string id)
        {
            var res = new Share().ChangeStatusUser(id);
            return Json(new
            {
                status = res
            });
        }

        [HttpGet]
        public ActionResult Create()
        {
            SetViewBagG();
            SetViewBagFaculty();
            return View();
        }

        public void SetViewBagG(string selectedID = null)
        {
            SelectList GenGender = new SelectList(new[] {
                new {Text = "Nam", Value = true},
                new {Text = "Nữ", Value = false},
            }, "Value", "Text");
            ViewBag.GenGender = GenGender;
        }

        public void SetViewBagFaculty(string selectedID = null)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            //var id = Session["Person"].ToString();
            var id = "ZXCVBUML";
            var list = database.Organization.Where(x => x.PersonID == id).ToList();
            var companyID = database.Person.Find(id).CompanyID;
            list.Remove(list.SingleOrDefault(x => x.ID == companyID));
            SelectList ListSchool = new SelectList(list, "ID", "Name");
            ViewBag.Faculty = ListSchool;
        }

        [HttpPost]
        public ActionResult Create(Person Representative)
        {
            if (ModelState.IsValid)
            {
                Person person = new Person();
                string personID;
                do
                {
                    personID = new Share().RandomText();
                } while (new Share().FindPerson(personID) == false);
                person.PersonID = personID;
                person.RoleID = 3;
                person.LastName = Representative.LastName;
                person.FirstName = Representative.FirstName;
                person.Birthday = Representative.Birthday;
                person.Gender = Representative.Gender;
                person.Address = Representative.Address;
                person.Phone = Representative.Phone;
                person.Email = Representative.Email;
                //person.CompanyID = Session["CompanyID"].ToString();
                person.CompanyID = "ASDFGHJR";
                person.SchoolID = Representative.SchoolID;
                if (new Share().InsertPerson(person))
                {
                    if (SendMailTK(personID))
                    {
                        ModelState.AddModelError("", "Thêm Giáo vụ thành công");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Không thể gửi Email kích hoạt");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Thêm Giáo vụ thất bại");
                }
                SetViewBagFaculty();
                SetViewBagG();
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
            var model = database.Person.Find(id);
            SetViewBagG();
            SetViewBagFaculty();
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Person Representative)
        {
            if (ModelState.IsValid)
            {
                var model1 = new Share().Update(Representative);
                if (model1)
                {
                    ModelState.AddModelError("", "Cập nhật thành công");
                }
                else
                {
                    ModelState.AddModelError("", "Cập nhật thất bại");
                }
            }
            SetViewBagG();
            SetViewBagFaculty();
            return View("Edit");
        }

        // Chú ý Method này được gọi trong file JS admin
        [HttpPost]
        public ActionResult Delete(string id)
        {
            JsonSerializerSettings json = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var result = "";
            var re = new Share().DeletePerson(id);
            if (re)
            {
                result = JsonConvert.SerializeObject("Xóa thành công", Formatting.Indented, json);
            }
            else
            {
                result = JsonConvert.SerializeObject("Xóa thất bại", Formatting.Indented, json);
            }
            return this.Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}