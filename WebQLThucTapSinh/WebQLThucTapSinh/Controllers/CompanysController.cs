using Newtonsoft.Json;
using QLThucTapSinh.Common;
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

        [HttpGet]
        public ActionResult Edit(string id)
        {
            //if (id == null)
            //{
            //    id = Session["CompanyID"].ToString();
            //}
            WebDatabaseEntities database = new WebDatabaseEntities();
            var model = database.Organization.SingleOrDefault(x => x.ID == id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Organization organi)
        {
            if (ModelState.IsValid)
            {

                var model1 = new CompanyAndSchool().Update(organi);
                if (model1)
                {
                    var modle = new CompanyAndSchool().listOrgan(2).OrderByDescending(x => x.StartDay).ToList();
                    return RedirectToAction("Index", modle);
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
            
            if (DeleteIntern(id) == true)
            {
                if (DeleteQuestion(id) == true)
                {
                    if (DeleteTask(id) == true)
                    {
                        if (DeleteInternship(id) == true)
                        {
                            if (DeleteLedder(id) == true)
                            {
                                WebDatabaseEntities database = new WebDatabaseEntities();
                                database.Person.Remove(database.Person.SingleOrDefault(x => x.CompanyID == id && x.RoleID == 2));
                                var model = database.Organization.Find(id);
                                database.Organization.Remove(model);
                                database.SaveChanges();
                            }
                        }
                    }
                }
            }
            JsonSerializerSettings json = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var result = JsonConvert.SerializeObject("Xóa thành công", Formatting.Indented, json);
            return this.Json(result, JsonRequestBehavior.AllowGet);
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

        public bool DeleteIntern(string id)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                var listIntern = database.Person.Where(x => x.CompanyID == id && x.RoleID == 5).ToList();
                foreach (var item in listIntern)
                {

                    database.Intern.Remove(database.Intern.Find(item.PersonID));
                    database.Users.Remove(database.Users.SingleOrDefault(x => x.PersonID == item.PersonID));
                    database.Person.Remove(item);
                    database.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }

        }

        public bool DeleteQuestion(string id)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                var ListQuestion = (from a in database.Person
                                    join b in database.Task on a.PersonID equals b.PersonID
                                    join c in database.Question on b.TaskID equals c.TaskID
                                    where a.CompanyID == id
                                    select new QuestionClass()
                                    {
                                        QuestionID = c.QuestionID
                                    }).ToList();
                foreach (var item in ListQuestion)
                {
                    database.Question.Remove(database.Question.Find(item.QuestionID));
                    database.SaveChanges();
                }

                return true;
            }
            catch
            {
                return false;
            }

        }

        public bool DeleteTask(string id)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                var ListTask = (from a in database.Person
                                join b in database.Organization on a.CompanyID equals b.ID
                                join c in database.Task on a.PersonID equals c.PersonID
                                where a.CompanyID == id
                                select new TaskClass()
                                {
                                    TaskID = c.TaskID
                                }).ToList();
                foreach (var item in ListTask)
                {
                    database.Task.Remove(database.Task.Find(item.TaskID));
                    database.SaveChanges();
                }

                return true;
            }
            catch
            {
                return false;
            }

        }

        public bool DeleteInternship(string id)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                var ListInternship = database.InternShip.Where(x => x.CompanyID == id).ToList();
                foreach (var item in ListInternship)
                {
                    database.InternShip.Remove(item);
                    var ListInternship2 = database.IntershipWithTask.Where(x => x.InternshipID == item.InternshipID).ToList();
                    foreach (var item2 in ListInternship2)
                    {
                        database.IntershipWithTask.Remove(item2);
                        database.SaveChanges();
                    }
                    database.SaveChanges();
                }

                return true;
            }
            catch
            {
                return false;
            }

        }

        public bool DeleteLedder(string id)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                var listLedder = database.Person.Where(x => x.CompanyID == id && x.RoleID == 4).ToList();
                foreach (var item in listLedder)
                {
                    database.Users.Remove(database.Users.SingleOrDefault(x => x.PersonID == item.PersonID));
                    database.Person.Remove(item);
                    database.SaveChanges();
                }

                return true;
            }
            catch
            {
                return false;
            }

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